#if USE_ADMOB
using System;
using System.Threading.Tasks;
using GoogleMobileAds.Api;
using UnityEngine;

namespace MidasTouch.AD.AdMob
{
    public class RewardedAdSnippets
    {
        private readonly string _adUnitId;
        private RewardedAd _rewardedAd;

        public RewardedAdSnippets(string adUnitId)
        {
            _adUnitId = adUnitId;
            _ = LoadNewAd();
        }

        private bool CanShow()
        {
            if (_rewardedAd == null || !_rewardedAd.CanShowAd()) return false;
            return true;
        }

        private async Task LoadNewAd()
        {
            while (true)
            {
                if (_rewardedAd == null)
                {
                    var adRequest = new AdRequest();

                    // Send the request to load the ad.
                    RewardedAd.Load(_adUnitId, adRequest, (ad, error) =>
                    {
                        if (error != null)
                        {
                            Debug.LogWarning(error);
                            return;
                        }

                        _rewardedAd = ad;
                    });
                }

                await Task.Delay(1000);
            }
        }

        public void ShowAd(Action<Reward> succeed, Action failed)
        {
            if (!CanShow()) return;

            ListenToAdEvents(_rewardedAd, failed);
            _rewardedAd?.Show(reward =>
            {
                succeed?.Invoke(reward);
                DestroyAd();
            });
        }

        private void ListenToAdEvents(RewardedAd rewardedAd, Action failed)
        {
            rewardedAd.OnAdPaid += adValue =>
            {
                // Raised when the ad is estimated to have earned money.
            };
            rewardedAd.OnAdImpressionRecorded += () =>
            {
                // Raised when an impression is recorded for an ad.
            };
            rewardedAd.OnAdClicked += () =>
            {
                // Raised when a click is recorded for an ad.
            };
            rewardedAd.OnAdFullScreenContentOpened += () =>
            {
                // Raised when the ad opened full screen content.
            };
            rewardedAd.OnAdFullScreenContentClosed += DestroyAd;
            rewardedAd.OnAdFullScreenContentFailed += error =>
            {
                Debug.LogWarning(error);
                failed?.Invoke();
                DestroyAd();
            };
        }

        private void DestroyAd()
        {
            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }
        }
    }
}
#endif