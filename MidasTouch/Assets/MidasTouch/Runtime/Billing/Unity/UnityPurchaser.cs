using System;
using UnityEngine.Purchasing;

namespace MidasTouch.Billing.Unity
{
    public class UnityPurchaser
    {
        private readonly StoreController _storeController;
        private readonly string _itemId;
        private readonly Action<bool> _callback;

        private UnityPurchaser(StoreController storeController, string itemId, Action<bool> callback)
        {
            _storeController = storeController;
            _itemId = itemId;
            _callback = callback;

            _storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            _storeController.OnPurchaseDeferred += OnPurchaseDeferred;
            _storeController.OnPurchaseFailed += OnPurchaseFailed;
            _storeController.OnPurchasePending += OnPurchasePending;
        }

        private void OnPurchaseConfirmed(Order obj)
        {
            _callback?.Invoke(true);
        }

        private void OnPurchaseDeferred(DeferredOrder obj)
        {
            throw new NotImplementedException();
        }

        private void OnPurchaseFailed(FailedOrder obj)
        {
            _callback?.Invoke(false);
        }

        private void OnPurchasePending(PendingOrder obj)
        {
            throw new NotImplementedException();
        }
    }
}