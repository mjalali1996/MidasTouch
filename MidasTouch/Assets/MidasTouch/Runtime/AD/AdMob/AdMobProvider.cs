using System;
using UnityEngine;
#if MIDASTOUCH_ADMOB
using GoogleMobileAds.Api;
#endif

namespace MidasTouch.AD.AdMob
{
    internal class AdMobProvider : IAdProvider
    {
        public bool BannerSupported => true;

        private readonly string _bannerAdId;
        private readonly string _interstitialAdId;
        private readonly string _rewardedAdId;

        private bool _initialized;
#if MIDASTOUCH_ADMOB
        private BannerSnippets _bannerSnippets;
        private InterstitialSnippets _interstitialSnippets;
        private RewardedAdSnippets _rewardedAdSnippets;
#endif

        internal AdMobProvider(AdMobConfig config)
        {
            _bannerAdId = config.BannerUnitId;
            _interstitialAdId = config.InterstitialUnitId;
            _rewardedAdId = config.RewardedAdUnitId;
        }

        public void Initialize(Action<bool> callback)
        {
#if MIDASTOUCH_ADMOB
            Debug.Log("Initializing AdMob Provider");
            MobileAds.Initialize(_ =>
            {
                _initialized = true;
                _bannerSnippets = new BannerSnippets(_bannerAdId);
                _interstitialSnippets = new InterstitialSnippets(_interstitialAdId);
                _rewardedAdSnippets = new RewardedAdSnippets(_rewardedAdId);
                callback?.Invoke(true);
            });
#endif
        }

        public void SetBannerActive(bool show)
        {
            if (!BannerSupported) return;
            if (!_initialized) return;

#if MIDASTOUCH_ADMOB
            if (show)
                _bannerSnippets.ShowBanner();
            else
                _bannerSnippets.HideBanner();
#endif
        }

        public void ShowInterstitial()
        {
            if (!_initialized) return;
#if MIDASTOUCH_ADMOB
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

#if MIDASTOUCH_ADMOB
            _rewardedAdSnippets.ShowAd(reward => success?.Invoke(true), () => success?.Invoke(false));
#endif
        }
    }
}