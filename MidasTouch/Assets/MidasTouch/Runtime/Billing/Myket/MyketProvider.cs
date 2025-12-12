using System;
using System.Collections.Generic;
using System.Linq;
using MidasTouch.Billing.Models;

#if MIDASTOUCH_MYKET
using MyketPlugin;
#endif
using Product = MidasTouch.Billing.Models.Product;

namespace MidasTouch.Billing.Myket
{
    public class MyketProvider : IBillingProvider, IDisposable
    {
        private readonly Product[] _products;
        public IReadOnlyList<Product> Products => _products;

#if MIDASTOUCH_MYKET
        private MyketConsumeHandler _consumeHandler;
        private MyketPurchaseHandler _purchaseHandler;

        private readonly BillingConfig _config;

        private bool _initialized;
        private Action<bool> _initializedCallback;

        internal MyketProvider(BillingConfig config)
        {
            _config = config;
            _products = config.Products.ToArray();

            IABEventManager.billingSupportedEvent += BillingSupportedEvent;
            IABEventManager.billingNotSupportedEvent += BillingNotSupportedEvent;
        }

        public void Initialize(Action<bool> callback)
        {
            if (_initialized)
            {
                callback?.Invoke(true);
                return;
            }

            if (_initializedCallback != null)
            {
                callback?.Invoke(false);
                return;
            }

            _initializedCallback = callback;
            MyketIAB.init(_config.MyketConfig.RsaKey);
        }

        public void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            _purchaseHandler.GetPurchases(purchases =>
            {
                var items = purchases.Select(p => new PurchasedItem()
                {
                    PurchaseToken = p.PurchaseToken,
                    ItemId = p.ProductId,
                    State = GetPurchaseState(p.PurchaseState)
                }).ToList();
                callback?.Invoke(items);
            });
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsdCallback)
        {
            _purchaseHandler.GetPurchases(purchases =>
            {
                var items = purchases.Where(p => p.PurchaseState == MyketPurchase.MyketPurchaseState.Purchased)
                    .Select(p => p.ProductId).ToArray();

                _consumeHandler.Consume(items, consumedItems =>
                {
                    var purchasedItems = consumedItems.Select(c => new PurchasedItem()
                    {
                        PurchaseToken = c.PurchaseToken,
                        ItemId = c.ProductId,
                        State = GetPurchaseState(c.PurchaseState)
                    }).ToList();
                    
                    consumedItemsdCallback?.Invoke(purchasedItems);
                });
            });
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            _purchaseHandler.Purchase(itemId, success);
        }

        public void Dispose()
        {
            IABEventManager.billingSupportedEvent -= BillingSupportedEvent;
            IABEventManager.billingNotSupportedEvent -= BillingNotSupportedEvent;
            _consumeHandler?.Dispose();
            _purchaseHandler?.Dispose();
            MyketIAB.unbindService();
        }

        private PurchaseState GetPurchaseState(MyketPurchase.MyketPurchaseState argPurchaseState)
        {
            switch (argPurchaseState)
            {
                case MyketPurchase.MyketPurchaseState.Purchased:
                    return PurchaseState.Purchased;
                case MyketPurchase.MyketPurchaseState.Refunded:
                    return PurchaseState.Refunded;
                case MyketPurchase.MyketPurchaseState.Canceled:
                default:
                    throw new ArgumentOutOfRangeException(nameof(argPurchaseState), argPurchaseState, null);
            }
        }

        private void BillingSupportedEvent()
        {
            _initialized = true;
            _consumeHandler = new MyketConsumeHandler();
            _purchaseHandler = new MyketPurchaseHandler(_consumeHandler);

            _initializedCallback?.Invoke(true);
            _initializedCallback = null;
        }

        private void BillingNotSupportedEvent(string obj)
        {
            _initialized = false;
            _initializedCallback?.Invoke(false);
            _initializedCallback = null;
        }
#else

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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
#endif
    }
}