using System.Threading.Tasks;
using Bazaar.Poolakey.Data;

namespace MidasTouch.Billing.Bazaar
{
    internal class ServerSideBazaar : BazaarProvider
    {
        private readonly string _webhookAddress;

        public ServerSideBazaar(string key, string webhookAddress) : base(key)
        {
            _webhookAddress = webhookAddress;
        }

        protected override async Task<bool> Consume(string itemId, string purchaseToken, SKUDetails.Type type)
        {
            return await Webhook.Consume(_webhookAddress, "BAZAAR", itemId, purchaseToken, GetItemType(type));
        }
    }
}