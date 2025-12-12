using System;
using System.Collections.Generic;
using System.Linq;

#if MIDASTOUCH_MYKET
using MyketPlugin;


namespace MidasTouch.Billing.Myket
{
    public class MyketConsumeHandler
    {
        private readonly List<MyketPurchase> _consumedIds = new();

        private string[] _productids;
        private Action<MyketPurchase[]> _consumedIdsCallback;

        private int _notConsumedItemsCount = 0;

        internal MyketConsumeHandler()
        {
            IABEventManager.consumePurchaseSucceededEvent += ConsumePurchaseSucceededEvent;
            IABEventManager.consumePurchaseFailedEvent += ConsumeFailedEvent;
        }

        public void Consume(string[] productIds, Action<MyketPurchase[]> consumedItems)
        {
            if (_consumedIdsCallback == null)
            {
                _consumedIdsCallback?.Invoke(Array.Empty<MyketPurchase>());
                return;
            }

            _consumedIdsCallback = consumedItems;
            _productids = productIds;
            _consumedIds.Clear();
            _notConsumedItemsCount = 0;
            MyketIAB.consumeProducts(productIds);
        }

        public void Dispose()
        {
            IABEventManager.consumePurchaseSucceededEvent -= ConsumePurchaseSucceededEvent;
            IABEventManager.consumePurchaseFailedEvent -= ConsumeFailedEvent;
        }

        private void ConsumePurchaseSucceededEvent(MyketPurchase obj)
        {
            if (_consumedIdsCallback == null) return;
            if (!_productids.Contains(obj.ProductId)) return;
            _consumedIds.Add(obj);

            TryCallBack();
        }

        private void ConsumeFailedEvent(string obj)
        {
            if (_consumedIdsCallback == null) return;
            _notConsumedItemsCount++;

            TryCallBack();
        }

        private void TryCallBack()
        {
            if (_consumedIdsCallback == null) return;
            if (_consumedIds.Count + _notConsumedItemsCount != _productids.Length) return;


            _consumedIdsCallback?.Invoke(_consumedIds.ToArray());
            _consumedIdsCallback = null;
        }
    }
}
#endif