using System;

namespace MidasTouch.AD
{
    public class ProxyAdProvider : IAdProvider
    {
        public bool BannerSupported { get; }

        private IAdProvider _adProvider;

        public ProxyAdProvider()
        {
#if USE_ADMOB
            // _adProvider = new 
#elif USE_TAPSELL
            _adProvider = new
#endif
        }

        public void Initialize(Action<bool> callback) => _adProvider.Initialize(callback);

        public void SetBannerActive(bool show) => _adProvider.SetBannerActive(show);

        public void ShowInterstitial() => _adProvider.ShowInterstitial();

        public void ShowRewarded(Action<bool> success) => _adProvider.ShowRewarded(success);
    }
}