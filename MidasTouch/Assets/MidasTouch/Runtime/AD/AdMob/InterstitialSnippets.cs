#if USE_ADMOB
using System.Collections.Generic;
using System.Linq;
using GoogleMobileAds.Api;
using UnityEngine;

namespace MidasTouch.AD.AdMob
{
    public class InterstitialSnippets
    {
        private readonly string _adUnitId;
        private readonly List<InterstitialAd> _interstitialAds;

        public InterstitialSnippets(string adUnitId)
        {
            _adUnitId = adUnitId;
            _interstitialAds = new List<InterstitialAd>();

            LoadNewAd();
        }

        private void LoadNewAd()
        {
            var adRequest = new AdRequest();

            // Send the request to load the ad.
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogWarning(error);
                    return;
                }

                _interstitialAds.Add(ad);
                ListenToAdEvents(ad);
            });
        }

        public bool CanShowAd()
        {
            var interstitialAd = _interstitialAds.LastOrDefault();
            return CanShow(interstitialAd);
        }

        private bool CanShow(InterstitialAd interstitialAd)
        {
            if (interstitialAd == null || !interstitialAd.CanShowAd()) return false;
            return true;
        }

        public void ShowAd()
        {
            var interstitialAd = _interstitialAds.LastOrDefault();
            if (!CanShow(interstitialAd)) return;

            interstitialAd?.Show();
        }

        private void ListenToAdEvents(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            interstitialAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                LoadNewAd();
                DestroyAd(interstitialAd);
            };
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogWarning(error);
                LoadNewAd();
                DestroyAd(interstitialAd);
            };
        }

        private void DestroyAd(InterstitialAd interstitialAd)
        {
            if (interstitialAd != null)
            {
                _interstitialAds.Remove(interstitialAd);
                interstitialAd.Destroy();
            }
        }
    }
}
#endif