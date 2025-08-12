using MidasTouch.AD;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Scripts
{
    public class SampleScene : MonoBehaviour
    {
        [SerializeField] private Button _bannerButton;
        [SerializeField] private Button _interstitialButton;
        [SerializeField] private Button _rewardedAdButton;

        private IAdProvider _adProvider;

        private bool _banner;

        private void Awake()
        {
            _bannerButton.onClick.AddListener(OnBannerButtonClicked);
            _interstitialButton.onClick.AddListener(OnInterstitialButtonClicked);
            _rewardedAdButton.onClick.AddListener(OnRewardedAdButtonClicked);
            _adProvider = new ProxyAdProvider();

            _adProvider.Initialize(b => { Debug.Log(b ? "Ad Provider Initialized" : "Ad Provider Not Initialized"); });
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