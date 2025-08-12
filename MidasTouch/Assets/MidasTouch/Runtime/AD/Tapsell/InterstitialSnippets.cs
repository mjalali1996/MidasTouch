#if USE_TAPSELL
using System.Threading.Tasks;
using TapsellPlusSDK;
using UnityEngine;

namespace MidasTouch.AD.Tapsell
{
    internal class InterstitialSnippets
    {
        private readonly string _zoneId;
        private string _responseId;

        public InterstitialSnippets(string zoneId)
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
                    TapsellPlus.RequestInterstitialAd(_zoneId, model => { _responseId = model.responseId; },
                        error => { Debug.Log(error.message); });

                await Task.Delay(1000);
            }
        }

        public void ShowAd()
        {
            if (!CanShowAd()) return;

            _responseId = string.Empty;
            TapsellPlus.ShowInterstitialAd(_responseId, null, null, null);
        }
    }
}
#endif