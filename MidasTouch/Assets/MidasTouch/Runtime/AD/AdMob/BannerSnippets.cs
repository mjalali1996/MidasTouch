using GoogleMobileAds.Api;

namespace MidasTouch.AD.AdMob
{
    public class BannerSnippets
    {
        private readonly string _adUnitId;
        private BannerView _bannerView;

        public BannerSnippets(string adUnitId)
        {
            _adUnitId = adUnitId;

        }

        private void LoadBanner()
        {
            _bannerView ??= new BannerView(_adUnitId, AdSize.Banner, AdPosition.Bottom);
            
            var adRequest = new AdRequest();

            _bannerView.LoadAd(adRequest);
        }

        public void ShowBanner()
        {
            if(_bannerView == null) 
                LoadBanner();
            
            _bannerView?.Show();
        }

        public void HideBanner()
        {
            if(_bannerView == null) return;
            _bannerView.Hide();
        }
        
        public void DestroyBanner()
        {
            if (_bannerView == null) return;
            
            _bannerView.Destroy();
            _bannerView = null;
        }
    }
}