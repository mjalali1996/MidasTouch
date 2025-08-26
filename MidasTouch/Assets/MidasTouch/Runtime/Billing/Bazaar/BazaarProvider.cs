using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if MIDASTOUCH_BAZAAR
using Bazaar.Data;
using Bazaar.Poolakey;
using Bazaar.Poolakey.Data;
#endif
using MidasTouch.Billing.Models;
using UnityEngine;

namespace MidasTouch.Billing.Bazaar
{
    internal abstract class BazaarProvider : IBillingProvider, IDisposable
    {
#if MIDASTOUCH_BAZAAR
        private Payment _payment;
        protected Payment Payment => _payment;

        private bool _initialized;
        public bool Initialized => _initialized;

        // private readonly List<SKUDetails> BazaarSkuDetails = new();
        private readonly Product[] _products;
        public IReadOnlyList<Product> Products => _products;

        internal BazaarProvider(BillingConfig config)
        {
            _products = config.Products.ToArray();
            var securityCheck = SecurityCheck.Enable(config.BazaarConfig.RsaKey);
            var paymentConfiguration = new PaymentConfiguration(securityCheck);
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

                if (!result.data)
                {
                    Debug.LogWarning(result.message);
                    callback?.Invoke(false);
                    return;
                }

                // await UpdateSkus(_skus);

                _initialized = true;
                callback(true);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                callback?.Invoke(false);
            }
        }

        public async void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            try
            {
                if (!_initialized)
                {
                    Debug.LogWarning("Bazaar is not initialized");
                    callback?.Invoke(new List<PurchasedItem>());
                    return;
                }

                var items = await GetPurchasesInfo();
                var purchasedItems = items.Select(i => new PurchasedItem()
                {
                    ItemId = i.productId,
                    PurchaseToken = i.purchaseToken,
                    State = GetPurchaseState(i.purchaseState)
                }).ToList();

                callback?.Invoke(purchasedItems);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                callback?.Invoke(new List<PurchasedItem>());
            }
        }

        public async void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            try
            {
                if (!_initialized)
                {
                    Debug.LogWarning("Bazaar is not initialized");
                    consumedItemsCallback?.Invoke(new List<PurchasedItem>());
                    return;
                }

                var items = await GetPurchasesInfo();
                var consumedItems = new List<PurchasedItem>();

                Debug.Log("Not consumed purchases:");
                foreach (var purchasedItem in items)
                {
                    Debug.Log($"{purchasedItem}");
                    var res = await Consume(purchasedItem.productId, purchasedItem.purchaseToken,
                        GetBazaarItemType(purchasedItem.productId));
                    if (!res) continue;
                    consumedItems.Add(new PurchasedItem()
                    {
                        ItemId = purchasedItem.productId,
                        PurchaseToken = purchasedItem.purchaseToken,
                        State = PurchaseState.Consumed
                    });
                }
                Debug.Log("----------------------------------------------------------------------------");

                consumedItemsCallback?.Invoke(consumedItems);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                consumedItemsCallback?.Invoke(new List<PurchasedItem>());
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

                var consumed = await Consume(itemId, result.data.purchaseToken, type);
                success?.Invoke(consumed);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                success?.Invoke(false);
            }
        }

        protected abstract Task<bool> Consume(string itemId, string purchaseToken, SKUDetails.Type type);

        public void Dispose()
        {
            if (_payment != null)
            {
                _payment.Disconnect();
                _payment = null;
            }
        }

        private async Task<List<PurchaseInfo>> GetPurchasesInfo()
        {
            var result = await _payment.GetPurchases();

            return result.data.Where(p => p.purchaseState == PurchaseInfo.State.Purchased).ToList();
        }

        private SKUDetails.Type GetBazaarItemType(string productId)
        {
            var product = _products.First(p=>p.ProductId == productId);
            return GetBazaarItemType(product.ItemType);
        }

        public static PurchaseState GetPurchaseState(PurchaseInfo.State state)
        {
            return state switch
            {
                PurchaseInfo.State.Purchased => PurchaseState.Purchased,
                PurchaseInfo.State.Refunded => PurchaseState.Refunded,
                PurchaseInfo.State.Consumed => PurchaseState.Consumed,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static SKUDetails.Type GetBazaarItemType(ItemType itemType)
        {
            var state = itemType switch
            {
                ItemType.Consumable => SKUDetails.Type.inApp,
                ItemType.Subscription => SKUDetails.Type.subscription,
                _ => throw new ArgumentOutOfRangeException()
            };
            return state;
        }

        public static ItemType GetItemType(SKUDetails.Type type)
        {
            var state = type switch
            {
                SKUDetails.Type.inApp => ItemType.Consumable,
                SKUDetails.Type.subscription => ItemType.Subscription,
                _ => throw new ArgumentOutOfRangeException()
            };
            return state;
        }
#else
        public IReadOnlyList<Product> Products { get; }

        public void Initialize(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void GetPurchases(Action<List<PurchasedItem>> callback)
        {
            throw new NotImplementedException();
        }

        public void TryConsumePreviousPurchases(Action<List<PurchasedItem>> consumedItemsCallback)
        {
            throw new NotImplementedException();
        }

        public void Purchase(string itemId, ItemType itemType, Action<bool> success)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
#endif
    }
}