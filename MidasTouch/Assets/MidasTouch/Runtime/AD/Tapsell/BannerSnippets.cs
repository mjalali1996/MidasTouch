#if MIDASTOUCH_TAPSELL
using System.Threading.Tasks;
using TapsellPlusSDK;
using TapsellPlusSDK.models;
using UnityEngine;

namespace MidasTouch.AD.Tapsell
{
    internal class BannerSnippets
    {
        private readonly string _zoneId;
        private readonly int _horizontalGravity;
        private readonly int _verticalGravity;
        private string _responseId;

        public BannerSnippets(string zoneId, int horizontalGravity, int verticalGravity)
        {
            _zoneId = zoneId;
            _horizontalGravity = horizontalGravity;
            _verticalGravity = verticalGravity;
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
                if (!string.IsNullOrEmpty(_responseId)) return;
                
                TapsellPlus.RequestStandardBannerAd(_zoneId,BannerType.Banner320X50 , model => { _responseId = model.responseId; },
                    error => { Debug.Log(error.message); });

                await Task.Delay(1000);
            }
        }

        public void ShowBanner()
        {
            if (!CanShowAd()) return;

            TapsellPlus.ShowStandardBannerAd(_responseId, _horizontalGravity, _verticalGravity, null, null);
        }

        public void HideBanner()
        {
            TapsellPlus.HideStandardBannerAd();
        }
    }
}
#endif