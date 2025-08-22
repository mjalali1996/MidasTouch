using System;
using System.Collections.Generic;
using MidasTouch.Billing.Bazaar;
using MidasTouch.Billing.Models;
using UnityEngine;

namespace MidasTouch.Billing
{
    public class BillingProxy : IBillingProvider
    {
        private readonly IBillingProvider _billingProvider;
        public IReadOnlyList<string> Products => _billingProvider.Products;

        public BillingProxy()
        {
#if MIDASTOUCH_BAZAAR
            var config = Resources.Load<BazaarConfig>(nameof(BazaarConfig));
            _billingProvider = new ClientSideBazaar(config);
#elif MIDASTOUCH_GOOGLEPLAY
#endif
        }


        public void Initialize(Action<bool> callback)
        {
            _billingProvider.Initialize(callback);
        }

        public void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            _billingProvider.GetPurchases(callback);
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            _billingProvider.TryConsumePreviousPurchases(consumedItemsCallback);
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            _billingProvider.Purchase(itemId, itemType, success);
        }
    }
}