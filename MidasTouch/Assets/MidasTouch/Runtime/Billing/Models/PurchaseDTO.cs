using System;

namespace MidasTouch.Billing.Models
{
    [Serializable]
    public class PurchaseDTO
    {
        public string Market;
        public string ItemId;
        public string PurchaseToken;
        public ItemType Type;
    }
}