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
            _adProvider = new AdMobProvider("ca-app-pub-3940256099942544/6300978111",
                "ca-app-pub-3940256099942544/1033173712", "ca-app-pub-3940256099942544/5224354917");
#elif USE_TAPSELL
            var testAppId = "alsoatsrtrotpqacegkehkaiieckldhrgsbspqtgqnbrrfccrtbdomgjtahflchkqtqosa";
            _adProvider = new TapsellProvider(testAppId, "5cfaaa30e8d17f0001ffb294", "5cfaa942e8d17f0001ffb292",
                "5cfaa802e8d17f0001ffb28e");
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