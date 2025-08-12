#if USE_TAPSELL
using System;
using System.Threading.Tasks;
using TapsellPlusSDK;
using UnityEngine;

namespace MidasTouch.AD.Tapsell
{
    internal class RewardedAdSnippets
    {
        private readonly string _zoneId;
        private string _responseId;

        public RewardedAdSnippets(string zoneId)
        {
            _zoneId = zoneId;
            _ = LoadNewAd();
        }


        public bool CanShowAd()
        {
            if (string.IsNullOrEmpty(_responseId)) return false;
            return true;
        }

        private async Task LoadNewAd()
        {
            while (true)
            {
                if (string.IsNullOrEmpty(_responseId))
                    TapsellPlus.RequestRewardedVideoAd(_zoneId, model => { _responseId = model.responseId; },
                        error => { Debug.Log(error.message); });

                await Task.Delay(2000);
            }
        }

        public void ShowAd(Action<bool> callback)
        {
            if (!CanShowAd()) return;

            _responseId = string.Empty;
            TapsellPlus.ShowRewardedVideoAd(_responseId, null, _ => callback?.Invoke(true),
                null,
                onShowError: _ => callback?.Invoke(false));
        }
    }
}
#endif