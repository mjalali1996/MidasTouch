#if USE_TAPSELL
using System;
using TapsellPlusSDK;
using UnityEngine;

namespace MidasTouch.AD.Tapsell
{
    internal class TapsellProvider : IAdProvider
    {
        private readonly string _tapsellPlusKey;
        public bool BannerSupported { get; }
        private bool _initialized;
        
        public TapsellProvider(string tapsellPlusKey)
        {
            _tapsellPlusKey = tapsellPlusKey;
        }

        public void Initialize(Action<bool> callback)
        {
            TapsellPlus.Initialize(_tapsellPlusKey,
                adNetworkName =>
                {
                    _initialized = true;
                    Debug.Log(adNetworkName + " Initialized Successfully.");
                    callback?.Invoke(true);
                },
                error =>
                {
                    _initialized = false;
                    Debug.Log(error.ToString());
                    callback?.Invoke(false);
                });
        }

        public void SetBannerActive(bool show)
        {
            throw new NotImplementedException();
        }

        public void ShowInterstitial()
        {
            // TapsellPlus.reward
        }

        public void ShowRewarded(Action<bool> success)
        {
            throw new NotImplementedException();
        }
    }
}
#endif