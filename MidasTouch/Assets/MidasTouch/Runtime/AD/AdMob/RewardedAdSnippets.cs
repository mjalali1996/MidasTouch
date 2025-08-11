#if USE_ADMOB
using System;
using System.Collections.Generic;
using System.Linq;
using GoogleMobileAds.Api;
using UnityEngine;

namespace MidasTouch.AD.AdMob
{
    public class RewardedAdSnippets
    {
        private readonly string _adUnitId;
        private readonly List<RewardedAd> _rewardedAds;

        public RewardedAdSnippets(string adUnitId)
        {
            _adUnitId = adUnitId;
            _rewardedAds = new List<RewardedAd>();
            LoadNewAd();
        }


        public bool CanShowAd()
        {
            var rewardedAd = _rewardedAds.LastOrDefault();
            return CanShow(rewardedAd);
        }

        private bool CanShow(RewardedAd rewardedAd)
        {
            if (rewardedAd == null || !rewardedAd.CanShowAd()) return false;
            return true;
        }

        private void LoadNewAd()
        {
            var adRequest = new AdRequest();

            RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    return;
                }

                _rewardedAds.Add(ad);
                ListenToAdEvents(ad);
            });
        }

        public void ShowAd(Action<Reward> callback)
        {
            var rewardedAd = _rewardedAds.LastOrDefault();

            if (!CanShow(rewardedAd)) return;

            rewardedAd?.Show(reward =>
            {
                callback?.Invoke(reward);
                DestroyAd(rewardedAd);
            });
        }

        private void ListenToAdEvents(RewardedAd rewardedAd)
        {
            rewardedAd.OnAdPaid += (AdValue adValue) =>
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
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                LoadNewAd();
                DestroyAd(rewardedAd);
            };
            rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogWarning(error);
                LoadNewAd();
                DestroyAd(rewardedAd);
            };
        }

        private void DestroyAd(RewardedAd rewardedAd)
        {
            if (rewardedAd != null)
            {
                _rewardedAds.Remove(rewardedAd);
                rewardedAd.Destroy();
            }
        }
    }
}
#endif