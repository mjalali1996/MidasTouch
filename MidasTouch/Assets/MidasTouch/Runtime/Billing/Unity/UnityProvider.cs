using System;
using System.Collections.Generic;
using MidasTouch.Billing.Models;

#if MIDASTOUCH_GOOGLEPLAY || MIDASTOUCH_APPLE
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using Product = UnityEngine.Purchasing.Product;
#endif

namespace MidasTouch.Billing.Unity
{
    public class UnityProvider : IBillingProvider
    {
#if MIDASTOUCH_GOOGLEPLAY || MIDASTOUCH_APPLE
        private StoreController _storeController;

        private readonly List<Models.Product> _products;
        public IReadOnlyList<Models.Product> Products => _products;
        private readonly List<Product> _unityProducts = new();
        private readonly CrossPlatformValidator _validator;

        private Product _purchasingProduct;
        private Action<bool> _purchaseCallback;

        private readonly List<PendingOrder> _previousPendingOrders = new();
        private readonly List<PendingOrder> _validPendingOrders = new();
        private readonly List<Order> _consumedItems = new();
        private Action<List<PurchasedItem>> _consumePreviousPurchasesCallback;


        public UnityProvider(BillingConfig config)
        {
            _products = config.Products.ToList();
            _validator = new CrossPlatformValidator(Array.Empty<byte>(), Array.Empty<byte>(), "", "");
        }

        public async void Initialize(Action<bool> callback)
        {
            try
            {
                _storeController = UnityIAPServices.StoreController();

                _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
                _storeController.OnPurchaseDeferred += OnPurchaseDeferred;
                _storeController.OnPurchaseFailed += OnPurchaseFailed;
                _storeController.OnPurchasePending += OnPurchasePending;

                await _storeController.Connect();

                var initialProductsToFetch = _products.Select(product =>
                    new ProductDefinition(product.ProductId, GetProductType(product.ItemType))).ToList();

                _storeController.OnProductsFetched += OnProductsFetched;
                _storeController.FetchProducts(initialProductsToFetch);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                callback?.Invoke(false);
            }
        }

        public void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            var purchasedItems = _previousPendingOrders.Select(p => new PurchasedItem()
            {
                ItemId = p.CartOrdered.Items().First().Product.definition.id,
                PurchaseToken = p.Info.Receipt,
                State = PurchaseState.Purchased,
            }).ToList();

            callback?.Invoke(purchasedItems);
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            if (_consumePreviousPurchasesCallback != null)
            {
                Debug.LogError("TryConsumePreviousPurchasesCallback is already in process");
                return;
            }

            _consumePreviousPurchasesCallback = consumedItemsCallback;
            _validPendingOrders.Clear();
            foreach (var pendingOrder in _previousPendingOrders)
            {
                var isValid = Validate(pendingOrder);
                if (!isValid) continue;
                _validPendingOrders.Add(pendingOrder);
            }

            _previousPendingOrders.Clear();

            foreach (var pendingOrder in _validPendingOrders)
            {
                _storeController.ConfirmPurchase(pendingOrder);
            }
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            if (IsPurchasing())
            {
                Debug.LogError("Another purchase is already in process");
                success?.Invoke(false);
                return;
            }

            _purchasingProduct = _unityProducts.First(p => p.definition.id == itemId);

            _storeController.PurchaseProduct(itemId);
            _purchaseCallback = success ?? (_ => { });
        }

        private void OnProductsFetched(List<Product> products)
        {
            _unityProducts.Clear();
            _unityProducts.AddRange(products);
        }

        private void OnPurchaseConfirmed(Order order)
        {
            var isPurchasingProduct = _purchasingProduct.definition.id ==
                                      order.CartOrdered.Items().First().Product.definition.id;
            if (isPurchasingProduct && IsPurchasing())
            {
                SetPurchasingResult(true);
                return;
            }

            var pendingOrder = _validPendingOrders.FirstOrDefault(p => p.Info.Receipt == order.Info.Receipt);
            if (pendingOrder != null && _consumePreviousPurchasesCallback != null)
            {
                _consumedItems.Add(order);
                TrySendConsumedOrderResult();
            }
        }

        private void OnPurchaseDeferred(DeferredOrder obj)
        {
            //Todo
        }

        private void OnPurchaseFailed(FailedOrder obj)
        {
            if (IsPurchasing())
            {
                Debug.LogWarning(obj.Details);
                SetPurchasingResult(false);
            }

            var pendingOrder = _validPendingOrders.FirstOrDefault(p => p.Info.Receipt == obj.Info.Receipt);
            if (pendingOrder != null && _consumePreviousPurchasesCallback != null)
            {
                _validPendingOrders.Remove(pendingOrder);
                TrySendConsumedOrderResult();
            }
        }

        private void OnPurchasePending(PendingOrder obj)
        {
            // If IsPurchasing() is false that's means OnPurchasePending called automatically by the Unity IAP package
            // for previous purchases that not confirm (consumed) yet, In other word, this let you confirm it again
            if (!IsPurchasing())
            {
                _previousPendingOrders.Add(obj);
                return;
            }

            var isValid = Validate(obj);

            if (isValid) _storeController.ConfirmPurchase(obj);
        }

        private bool IsPurchasing()
        {
            return _purchasingProduct != null;
        }

        private bool Validate(PendingOrder obj)
        {
            var receipts = _validator.Validate(obj.Info.Receipt);
            return receipts.Any(r => r.transactionID == obj.Info.TransactionID);
        }

        private void SetPurchasingResult(bool result)
        {
            if (!IsPurchasing()) return;
            try
            {
                _purchaseCallback?.Invoke(result);
            }
            finally
            {
                _purchaseCallback = null;
                _purchasingProduct = null;
            }
        }

        private void TrySendConsumedOrderResult()
        {
            if (_consumedItems.Count != _validPendingOrders.Count) return;
            try
            {
                var purchasedItems = _consumedItems.Select(o => new PurchasedItem()
                {
                    ItemId = o.CartOrdered.Items().First().Product.definition.id,
                    PurchaseToken = o.Info.Receipt,
                    State = PurchaseState.Consumed,
                }).ToList();
                _consumePreviousPurchasesCallback?.Invoke(purchasedItems);
            }
            finally
            {
                _consumePreviousPurchasesCallback = null;
                _consumedItems.Clear();
            }
        }

        private static ProductType GetProductType(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Consumable:
                    return ProductType.Consumable;
                case ItemType.Subscription:
                    return ProductType.Subscription;
                default:
                    throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
            }
        }

        private static ItemType GetItemType(ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Consumable:
                    return ItemType.Consumable;
                case ProductType.Subscription:
                    return ItemType.Subscription;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ProductType), productType, null);
            }
        }
#else
        public IReadOnlyList<Models.Product> Products { get; }
        public void Initialize(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            throw new NotImplementedException();
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            throw new NotImplementedException();
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            throw new NotImplementedException();
        }
#endif
    }
}