using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bazaar.Data;
using Bazaar.Poolakey;
using Bazaar.Poolakey.Data;
using UnityEngine;

namespace MidasTouch.Billing.Bazaar
{
    internal class BazaarProvider : IBillingProvider, IDisposable
    {
        private readonly IPurchaseTokenValidator _validator;
        private Payment _payment;
        private bool _initialized;

        internal BazaarProvider(string key, IPurchaseTokenValidator validator)
        {
            var securityCheck = SecurityCheck.Enable(key);
            var paymentConfiguration = new PaymentConfiguration(securityCheck);
            _validator = validator;
            _payment = new Payment(paymentConfiguration);
        }

        public async void Initialize(Action<bool> callback)
        {
            try
            {
                if (_initialized)
                {
                    callback?.Invoke(true);
                    return;
                }

                var result = await _payment.Connect();

                if (result.data) _initialized = true;

                callback(result.data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                callback?.Invoke(false);
            }
        }

        public async void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            var items = new List<PurchasedItem>();
            try
            {
                if (!_initialized)
                {
                    Debug.LogWarning("Bazaar is not initialized");
                    callback?.Invoke(items);
                    return;
                }
                var result = await _payment.GetPurchases();

                foreach (var purchaseInfo in result.data)
                {
                    items.Add(new PurchasedItem()
                    {
                        ItemId = purchaseInfo.productId,
                        PurchaseToken = purchaseInfo.purchaseToken,
                        State = GetPurchaseState(purchaseInfo),
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                callback?.Invoke(items);
            }
        }

        public async void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            try
            {
                if (!_initialized)
                {
                    Debug.LogWarning("Bazaar is not initialized");
                    success?.Invoke(false);
                    return;
                }

                var type = GetBazaarItemType(itemType);

                var result = await _payment.Purchase(itemId, type);

                if (result.status != Status.Success)
                {
                    Debug.LogWarning("Failed Bazaar Purchase");
                    success?.Invoke(false);
                    return;
                }

                var consumed = await Consume(result.data.purchaseToken, type);
                success?.Invoke(consumed);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                success?.Invoke(false);
            }
        }

        private async Task<bool> Consume(string purchaseToken, SKUDetails.Type type)
        {
            if (!_initialized)
            {
                Debug.LogWarning("Bazaar is not initialized");
                return false;
            }

            var state = await _validator.ValidateToken(purchaseToken);
            if (state is IPurchaseTokenValidator.State.Invalid or IPurchaseTokenValidator.State.Consumed)
            {
                Debug.LogWarning("Purchase Token is invalid");
                return false;
            }

            if (type == SKUDetails.Type.inApp)
            {
                var consumedResult = await _payment.Consume(purchaseToken);

                if (consumedResult.status != Status.Success)
                {
                    Debug.LogWarning("Failed to consume Bazaar item");
                    return false;
                }
            }

            var consumed = await _validator.Consume(purchaseToken);
            if (!consumed)
                Debug.LogWarning("Failed to consume item by validator");
            return consumed;
        }

        public void Dispose()
        {
            if (_payment != null)
            {
                _payment.Disconnect();
                _payment = null;
            }
        }

        private static PurchaseState GetPurchaseState(PurchaseInfo purchaseInfo)
        {
            var state = purchaseInfo.purchaseState switch
            {
                PurchaseInfo.State.Purchased => PurchaseState.Purchased,
                PurchaseInfo.State.Refunded => PurchaseState.Refunded,
                PurchaseInfo.State.Consumed => PurchaseState.Consumed,
                _ => throw new ArgumentOutOfRangeException()
            };
            return state;
        }

        private static SKUDetails.Type GetBazaarItemType(ItemType itemType)
        {
            var state = itemType switch
            {
                ItemType.Consumable => SKUDetails.Type.inApp,
                ItemType.Subscription => SKUDetails.Type.subscription,
                _ => throw new ArgumentOutOfRangeException()
            };
            return state;
        }
    }
}