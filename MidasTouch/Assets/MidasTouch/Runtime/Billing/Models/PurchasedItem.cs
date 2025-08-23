namespace MidasTouch.Billing.Models
{
    public struct PurchasedItem
    {
        public string ItemId;
        public string PurchaseToken;
        public PurchaseState State;

        public override string ToString()
        {
            return $"Id: {ItemId}, PurchaseToken: {PurchaseToken}, State: {State}";
        }
    }
}