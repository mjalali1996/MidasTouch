using System;
using System.Collections.Generic;

namespace MidasTouch.Billing
{
    public interface IBillingProvider
    {
        void Initialize(Action<bool> callback);
        void GetPurchases(Action<List<PurchasedItem>> callback);
        void Purchase(string itemId, ItemType itemType, Action<bool> success);
    }
}