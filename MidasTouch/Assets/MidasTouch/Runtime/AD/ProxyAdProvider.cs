using System;
using System.Data;

#if USE_ADMOB
using MidasTouch.AD.AdMob;
#elif USE_TAPSELL
using MidasTouch.AD.Tapsell;
#endif

namespace MidasTouch.AD
{
    public class ProxyAdProvider : IAdProvider
    {
        public bool BannerSupported
        {
            get
            {
                _ = CheckProvider(true);
                return _adProvider.BannerSupported;
            }
        }

        private readonly IAdProvider _adProvider;

        public ProxyAdProvider()
        {
#if USE_ADMOB
            // _adProvider = new 
#elif USE_TAPSELL
            _adProvider = new TapsellProvider("");
#endif
        }

        public void Initialize(Action<bool> callback)
        {
            _ = CheckProvider(true);

            _adProvider.Initialize(callback);
        }

        public void SetBannerActive(bool show)
        {
            _ = CheckProvider(true);
 
            _adProvider.SetBannerActive(show);
        }

        public void ShowInterstitial()
        {
            _ = CheckProvider(true);
 
            _adProvider.ShowInterstitial();
        }

        public void ShowRewarded(Action<bool> success)
        {
            _ = CheckProvider(true);
 
            _adProvider.ShowRewarded(success);
        }

        private bool CheckProvider(bool throwException = false)
        {
            if (_adProvider != null) return true;

            if (!throwException) return false;

            throw new ConstraintException();
        }
    }
}