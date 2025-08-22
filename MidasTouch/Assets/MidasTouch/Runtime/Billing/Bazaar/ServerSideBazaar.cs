#if MIDASTOUCH_BAZAAR
using System.Collections.Generic;
using System.Threading.Tasks;
using Bazaar.Poolakey.Data;
#endif

namespace MidasTouch.Billing.Bazaar
{
    internal class ServerSideBazaar : BazaarProvider
    {
#if MIDASTOUCH_BAZAAR
        private readonly string _webhookAddress;

        public ServerSideBazaar(BazaarConfig config) : base(config)
        {
            _webhookAddress = config.WebhookAddress;
        }

        protected override async Task<bool> Consume(string itemId, string purchaseToken, SKUDetails.Type type)
        {
            return await Webhook.Consume(_webhookAddress, "BAZAAR", itemId, purchaseToken, GetItemType(type));
        }
#endif
    }
}