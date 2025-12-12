using System;
using System.Collections.Generic;
using System.Data;
using MidasTouch.Billing.Bazaar;
using MidasTouch.Billing.Models;
using MidasTouch.Billing.Myket;
using MidasTouch.Billing.Unity;
using UnityEngine;

namespace MidasTouch.Billing
{
    public class BillingProxy : IBillingProvider
    {
        private readonly IBillingProvider _billingProvider;
        public IReadOnlyList<Product> Products => _billingProvider.Products;

        public BillingProxy()
        {
            var config = Resources.Load<BillingConfig>(nameof(BillingConfig));

#if MIDASTOUCH_BAZAAR
            _billingProvider = new ClientSideBazaar(config);
#elif MIDASTOUCH_MYKET
            _billingProvider = new MyketProvider(config);
#elif MIDASTOUCH_GOOGLEPLAY
            _billingProvider = new UnityProvider(config);
#elif MIDASTOUCH_APPLE
            _billingProvider = new UnityProvider(config);
#endif
        }


        public void Initialize(Action<bool> callback)
        {
            var valid = CheckProvider();
            if (!valid)
            {
                callback?.Invoke(false);
                return;
            }

            _billingProvider.Initialize(callback);
        }

        public void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            _ = CheckProvider(true);
            _billingProvider.GetPurchases(callback);
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            _ = CheckProvider(true);
            _billingProvider.TryConsumePreviousPurchases(consumedItemsCallback);
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            _ = CheckProvider(true);
            _billingProvider.Purchase(itemId, itemType, success);
        }

        private bool CheckProvider(bool throwException = false)
        {
            if (_billingProvider != null) return true;

            if (!throwException) return false;

            throw new ConstraintException();
        }
    }
}