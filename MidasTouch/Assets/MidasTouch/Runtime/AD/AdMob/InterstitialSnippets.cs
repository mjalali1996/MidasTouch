#if MIDASTOUCH_ADMOB
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

namespace MidasTouch.AD.AdMob
{
    public class InterstitialSnippets
    {
        private readonly string _adUnitId;
        private InterstitialAd _interstitialAd;

        public InterstitialSnippets(string adUnitId)
        {
            _adUnitId = adUnitId;

            _ = LoadNewAd();
        }

        private async Task LoadNewAd()
        {
            while (true)
            {
                if (_interstitialAd == null)
                {
                    var adRequest = new AdRequest();

                    // Send the request to load the ad.
                    InterstitialAd.Load(_adUnitId, adRequest, (ad, error) =>
                    {
                        if (error != null)
                        {
                            Debug.LogWarning(error);
                            return;
                        }

                        _interstitialAd = ad;
                    });
                }

                await Task.Delay(1000);
            }
        }

        private bool CanShow()
        {
            if (_interstitialAd == null || !_interstitialAd.CanShowAd()) return false;
            return true;
        }

        public void ShowAd()
        {
            if (!CanShow()) return;

            ListenToAdEvents(_interstitialAd);
            _interstitialAd?.Show();
        }

        private void ListenToAdEvents(InterstitialAd interstitialAd)
        {
            interstitialAd.OnAdPaid += adValue =>
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
            interstitialAd.OnAdFullScreenContentClosed += DestroyAd;
            interstitialAd.OnAdFullScreenContentFailed += error =>
            {
                Debug.LogWarning(error);
                DestroyAd();
            };
        }

        private void DestroyAd()
        {
            if (_interstitialAd != null)
            {
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }
        }
    }
}
#endif