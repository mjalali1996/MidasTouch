#if MIDASTOUCH_BAZAAR
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazaar.Data;
using Bazaar.Poolakey.Data;
using UnityEngine;
#endif

namespace MidasTouch.Billing.Bazaar
{
    internal class ClientSideBazaar : BazaarProvider
    {
#if MIDASTOUCH_BAZAAR
        internal ClientSideBazaar(BillingConfig config) : base(config)
        {
        }

        protected override async Task<bool> Consume(string itemId, string purchaseToken, SKUDetails.Type type)
        {
            if (!Initialized)
            {
                Debug.LogWarning("ClientSideBazaar has not been initialized yet");
                return false;
            }

            var purchases = await Payment.GetPurchases(type);

            var purchaseInfo = purchases.data.FirstOrDefault(pi =>
                pi.purchaseToken == purchaseToken && pi.purchaseState == PurchaseInfo.State.Purchased);
            if (purchaseInfo == null)
            {
                Debug.LogWarning("Purchase Token is invalid");
                return false;
            }

            if (type == SKUDetails.Type.subscription) return true;

            var consumedResult = await Payment.Consume(purchaseToken);
            if (consumedResult.status == Status.Success) return true;

            Debug.LogWarning("Failed to consume Bazaar item");
            return false;
        }
#endif
    }
}