using System;
using System.Collections.Generic;
using MidasTouch.Billing.Models;

namespace MidasTouch.Billing
{
    public interface IBillingProvider
    {
        public IReadOnlyList<Product> Products { get; }
        void Initialize(Action<bool> callback);
        void GetPurchases(Action<List<PurchasedItem>> callback);
        void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback);
        void Purchase(string itemId, ItemType itemType, Action<bool> success);
    }
}