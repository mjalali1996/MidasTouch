using System;

namespace MidasTouch.AD
{
    public interface IAdProvider
    {
        public bool BannerSupported { get; }
        public void Initialize(Action<bool> callback);
        public void SetBannerActive(bool show);
        public void ShowInterstitial();
        public void ShowRewarded(Action<bool> success);
    }
}
