using System;
using System.Collections.Generic;
using System.Linq;
#if MIDASTOUCH_MYKET
using MyketPlugin;

namespace MidasTouch.Billing.Myket
{
    public class MyketPurchaseHandler : IDisposable
    {
        private string _productId;
        private Action<bool> _purchaseCallback;
        private Action<MyketPurchase[]> _purchasesCallback;

        internal MyketPurchaseHandler(MyketConsumeHandler consumeHandler)
        {
            IABEventManager.purchaseSucceededEvent += PurchaseSucceededEvent;
            IABEventManager.consumePurchaseSucceededEvent += ConsumePurchaseSucceededEvent;
            IABEventManager.purchaseFailedEvent += PurchaseFailedEvent;
            IABEventManager.consumePurchaseFailedEvent += PurchaseFailedEvent;

            IABEventManager.queryPurchasesSucceededEvent += IABEventManagerOnqueryPurchasesSucceededEvent;
            IABEventManager.queryPurchasesFailedEvent += IABEventManagerOnqueryPurchasesFailedEvent;
        }

        public void GetPurchases(Action<MyketPurchase[]> purchases)
        {
            if (_purchasesCallback != null)
            {
                purchases?.Invoke(Array.Empty<MyketPurchase>());
                return;
            }

            _purchasesCallback = purchases;
            MyketIAB.queryPurchases();
        }

        public void Purchase(string itemId, Action<bool> success)
        {
            if (_purchaseCallback != null)
            {
                success?.Invoke(false);
                return;
            }

            _productId = itemId;
            _purchaseCallback = success;
            MyketIAB.purchaseProduct(itemId);
        }

        public void Dispose()
        {
            IABEventManager.purchaseSucceededEvent -= PurchaseSucceededEvent;
            IABEventManager.consumePurchaseSucceededEvent -= ConsumePurchaseSucceededEvent;
            IABEventManager.purchaseFailedEvent -= PurchaseFailedEvent;
            IABEventManager.consumePurchaseFailedEvent -= PurchaseFailedEvent;
        }

        private void PurchaseSucceededEvent(MyketPurchase obj)
        {
            if (obj.ProductId != _productId) return;
            if (_purchaseCallback == null) return;

            MyketIAB.consumeProduct(obj.ProductId);
        }

        private void ConsumePurchaseSucceededEvent(MyketPurchase obj)
        {
            if (obj.ProductId != _productId) return;
            if (_purchaseCallback == null) return;

            _purchaseCallback?.Invoke(true);
            _purchaseCallback = null;
        }

        private void PurchaseFailedEvent(string obj)
        {
            if (_purchaseCallback == null) return;

            _purchaseCallback?.Invoke(false);
            _purchaseCallback = null;
        }

        private void IABEventManagerOnqueryPurchasesSucceededEvent(List<MyketPurchase> obj)
        {
            if (_purchasesCallback == null) return;

            var items = obj.Where(o => o.PurchaseState != MyketPurchase.MyketPurchaseState.Canceled)
                .ToArray();

            _purchasesCallback?.Invoke(items);
            _purchasesCallback = null;
        }

        private void IABEventManagerOnqueryPurchasesFailedEvent(string obj)
        {
            if (_purchasesCallback == null) return;
            _purchasesCallback?.Invoke(Array.Empty<MyketPurchase>());
            _purchasesCallback = null;
        }
    }
}

#endif