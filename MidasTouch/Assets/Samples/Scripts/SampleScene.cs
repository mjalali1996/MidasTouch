using MidasTouch.AD;
using MidasTouch.Billing;
using MidasTouch.Billing.Models;
using UnityEditor.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Samples.Scripts
{
    public class SampleScene : MonoBehaviour
    {
        [Header("Ad")] [SerializeField] private Button _bannerButton;
        [SerializeField] private Button _interstitialButton;
        [SerializeField] private Button _rewardedAdButton;

        [Header("Billing")] [SerializeField] private Button _buyItem1Button;
        [SerializeField] private Button _buyItem2Button;
        [SerializeField] private Button _buySubscriptionButton;
        [SerializeField] private Button _consumePreviousPurchasesButton;

        private IAdProvider _adProvider;
        private BillingProxy _billingProvider;

        private bool _banner;

        private void Awake()
        {
            _bannerButton.onClick.AddListener(OnBannerButtonClicked);
            _interstitialButton.onClick.AddListener(OnInterstitialButtonClicked);
            _rewardedAdButton.onClick.AddListener(OnRewardedAdButtonClicked);
            _adProvider = new AdProviderProxy();

            _adProvider.Initialize(b => { Debug.Log(b ? "Ad Provider Initialized" : "Ad Provider Not Initialized"); });

            _buyItem1Button.onClick.AddListener(OnBuyItem1Clicked);
            _buyItem2Button.onClick.AddListener(OnBuyItem2Clicked);
            _buySubscriptionButton.onClick.AddListener(OnBuySubscriptionClicked);
            _consumePreviousPurchasesButton.onClick.AddListener(OnConsumePreviousPurchasesClicked);

            _billingProvider = new BillingProxy();
            _billingProvider.Initialize(b =>
            {
                Debug.Log(b ? "Billing Provider initialized" : "Billing Provider Not Initialized");
            });
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


        private void OnBuyItem1Clicked()
        {
            _billingProvider.Purchase(_billingProvider.Products[0].ProductId, ItemType.Consumable,
                b => { Debug.Log($"Item 1 Purchase {(b ? "Was" : "Was Not")} Successfully"); });
        }

        private void OnBuyItem2Clicked()
        {
            _billingProvider.Purchase(_billingProvider.Products[1].ProductId, ItemType.Consumable,
                b => { Debug.Log($"Item 2 Purchase {(b ? "Was" : "Was Not")} Successfully"); });
        }

        private void OnBuySubscriptionClicked()
        {
            _billingProvider.Purchase(_billingProvider.Products[2].ProductId, ItemType.Subscription,
                b => { Debug.Log($"Subscription {(b ? "Was" : "Was Not")} Successfully"); });
        }

        private void OnConsumePreviousPurchasesClicked()
        {
            _billingProvider.TryConsumePreviousPurchases(list =>
            {
                foreach (var purchasedItem in list)
                {
                    Debug.Log($"Consumed: {purchasedItem}");
                }
            });
        }
    }
}