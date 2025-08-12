#if USE_TAPSELL
using System;
using TapsellPlusSDK;
using TapsellPlusSDK.models;
using UnityEngine;

namespace MidasTouch.AD.Tapsell
{
    internal class TapsellProvider : IAdProvider
    {
        private readonly string _tapsellPlusKey;
        private readonly string _bannerZoneId;
        private readonly string _interstitialZoneId;
        private readonly string _rewardedZoneId;
        public bool BannerSupported => true;
        
        private bool _initialized;
        
        
        private BannerSnippets _bannerSnippets;
        private InterstitialSnippets _interstitialSnippets;
        private RewardedAdSnippets _rewardedAdSnippets;

        public TapsellProvider(string tapsellPlusKey, string bannerZoneId, string interstitialZoneId, string rewardedZoneId)
        {
            _tapsellPlusKey = tapsellPlusKey;
            _bannerZoneId = bannerZoneId;
            _interstitialZoneId = interstitialZoneId;
            _rewardedZoneId = rewardedZoneId;
        }

        public void Initialize(Action<bool> callback)
        {
            TapsellPlus.Initialize(_tapsellPlusKey,
                adNetworkName =>
                {
                    _initialized = true;
                    _bannerSnippets = new BannerSnippets(_bannerZoneId, Gravity.Center, Gravity.Bottom);
                    _interstitialSnippets = new InterstitialSnippets(_interstitialZoneId);
                    _rewardedAdSnippets = new RewardedAdSnippets(_rewardedZoneId);
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
            if (!BannerSupported) return;
            if (!_initialized) return;

            if (show)
                _bannerSnippets.ShowBanner();
            else
                _bannerSnippets.HideBanner();
        }

        public void ShowInterstitial()
        {
            if (!_initialized) return;
            _interstitialSnippets.ShowAd();
        }

        public void ShowRewarded(Action<bool> success)
        {
            if (!_initialized)
            {
                success?.Invoke(false);
                return;
            }

            _rewardedAdSnippets.ShowAd(success);
        }
    }
}
#endif