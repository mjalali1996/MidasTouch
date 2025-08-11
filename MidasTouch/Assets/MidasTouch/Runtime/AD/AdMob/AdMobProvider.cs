#if USE_ADMOB
using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace MidasTouch.AD.AdMob
{
    internal class AdMobProvider : IAdProvider
    {
        public bool BannerSupported => true;

        private readonly string _bannerAdId;
        private readonly string _interstitialAdId;
        private readonly string _rewardedAdId;

        private bool _initialized;
        private BannerSnippets _bannerSnippets;
        private InterstitialSnippets _interstitialSnippets;
        private RewardedAdSnippets _rewardedAdSnippets;


        internal AdMobProvider(string bannerAdId, string interstitialAdId, string rewardedAdId)
        {
            _bannerAdId = bannerAdId;
            _interstitialAdId = interstitialAdId;
            _rewardedAdId = rewardedAdId;
        }

        public void Initialize(Action<bool> callback)
        {
            Debug.Log("Initializing AdMob Provider");
            MobileAds.Initialize(_ =>
            {
                _initialized = true;
                _bannerSnippets = new BannerSnippets(_bannerAdId);
                _interstitialSnippets = new InterstitialSnippets(_interstitialAdId);
                _rewardedAdSnippets = new RewardedAdSnippets(_rewardedAdId);
                callback?.Invoke(true);
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

            _rewardedAdSnippets.ShowAd(reward => { success?.Invoke(true); });
        }
    }
}
#endif