using MidasTouch.AD;
using MidasTouch.Billing;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Scripts
{
    public class SampleScene : MonoBehaviour
    {
        [Header("Ad")]
        [SerializeField] private Button _bannerButton;
        [SerializeField] private Button _interstitialButton;
        [SerializeField] private Button _rewardedAdButton;

        [Header("Billing")]
        [SerializeField] private Button _buyItem1Button;
        [SerializeField] private Button _buyItem2Button;
        [SerializeField] private Button _buySubscriptionButton;
        
        private IAdProvider _adProvider;

        private bool _banner;

        private void Awake()
        {
            _bannerButton.onClick.AddListener(OnBannerButtonClicked);
            _interstitialButton.onClick.AddListener(OnInterstitialButtonClicked);
            _rewardedAdButton.onClick.AddListener(OnRewardedAdButtonClicked);
            _adProvider = new AdProviderProxy();

            _adProvider.Initialize(b => { Debug.Log(b ? "Ad Provider Initialized" : "Ad Provider Not Initialized"); });

            var billingProvider = new BillingProxy();
        }

        private void OnBannerButtonClicked()
        {
            _banner = !_banner;
            _adProvider.SetBannerActive(_banner);
        }

        private void OnInterstitialButtonClicked()
        {
            _adProvider.ShowInterstitial();
        }

        private void OnRewardedAdButtonClicked()
        {
            _adProvider.ShowRewarded(b => { Debug.Log(b ? "Rewarded Ad Was Successful" : "Rewarded Ad Failed"); });
        }
    }
}