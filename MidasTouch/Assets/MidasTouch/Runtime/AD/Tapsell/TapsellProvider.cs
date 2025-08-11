#if USE_TAPSELL
using System;

namespace MidasTouch.AD.Tapsell
{
    internal class TapsellProvider : IAdProvider
    {
        public bool BannerSupported { get; }
        public void Initialize(Action<bool> callback)
        {
            throw new NotImplementedException();
        }

        public void SetBannerActive(bool show)
        {
            throw new NotImplementedException();
        }

        public void ShowInterstitial()
        {
            throw new NotImplementedException();
        }

        public void ShowRewarded(Action<bool> success)
        {
            throw new NotImplementedException();
        }
    }
}
#endif