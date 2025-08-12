using System;
#if USE_TAPSELL
using TapsellPlusSDK;
using TapsellPlusSDK.models;
#endif
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


#if USE_TAPSELL
        private BannerSnippets _bannerSnippets;
        private InterstitialSnippets _interstitialSnippets;
        private RewardedAdSnippets _rewardedAdSnippets;
#endif
        public TapsellProvider(TapsellConfig config)
        {
            _tapsellPlusKey = config.AppId;
            _bannerZoneId = config.BannerZoneId;
            _interstitialZoneId = config.InterstitialZoneId;
            _rewardedZoneId = config.RewardedZoneId;
        }

        public void Initialize(Action<bool> callback)
        {
#if USE_TAPSELL
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
#endif
        }

        public void SetBannerActive(bool show)
        {
            if (!BannerSupported) return;
            if (!_initialized) return;

#if USE_TAPSELL
            if (show)
                _bannerSnippets.ShowBanner();
            else
                _bannerSnippets.HideBanner();
#endif
        }

        public void ShowInterstitial()
        {
            if (!_initialized) return;
#if USE_TAPSELL
            _interstitialSnippets.ShowAd();
#endif
        }

        public void ShowRewarded(Action<bool> success)
        {
            if (!_initialized)
            {
                success?.Invoke(false);
                return;
            }

#if USE_TAPSELL
            _rewardedAdSnippets.ShowAd(success);
#endif
        }
    }
}