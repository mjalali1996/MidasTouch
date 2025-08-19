namespace MidasTouch.Billing
{
    public struct PurchasedItem
    {
        public string ItemId;
        public string PurchaseToken;
        public PurchaseState State;
    }

    public enum ItemType
    {
        Consumable,
        Subscription,
    }

    public enum PurchaseState
    {
        Purchased = 0,
        Refunded = 1,
        Consumed = 2
    }
}