namespace MidasTouch.Billing.Models
{
    public struct PurchasedItem
    {
        public string ItemId;
        public string PurchaseToken;
        public PurchaseState State;
    }
}