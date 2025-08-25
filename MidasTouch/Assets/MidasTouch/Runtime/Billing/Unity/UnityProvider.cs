using System;
using System.Collections.Generic;
using System.Linq;
using MidasTouch.Billing.Models;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using Product = UnityEngine.Purchasing.Product;

namespace MidasTouch.Billing.Unity
{
    public class UnityProvider : IBillingProvider
    {
        private StoreController _storeController;

        private readonly List<Models.Product> _products;
        public IReadOnlyList<Models.Product> Products => _products;
        private readonly List<Product> _unityProducts = new();


        public UnityProvider(BillingConfig config)
        {
            _products = config.Products.ToList();
            var sec = new CrossPlatformValidator(Array.Empty<byte>(), Array.Empty<byte>(), "", "");
        }

        public async void Initialize(Action<bool> callback)
        {
            try
            {
                _storeController = UnityIAPServices.StoreController();

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
            callback?.Invoke(new List<PurchasedItem>());
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            consumedItemsCallback?.Invoke(new List<PurchasedItem>());
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            _storeController.PurchaseProduct(itemId);
            \
        }

        private void OnProductsFetched(List<Product> products)
        {
            _unityProducts.Clear();
            _unityProducts.AddRange(products);
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
    }
}