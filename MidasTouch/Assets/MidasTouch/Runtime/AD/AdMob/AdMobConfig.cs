using UnityEngine;

namespace MidasTouch.AD.AdMob
{
    [CreateAssetMenu(menuName = "MidasTouch/Configs/Ad/" + nameof(AdMobConfig), fileName = nameof(AdMobConfig))]
    public class AdMobConfig : ScriptableObject
    {
        private const string TestBannerUnitId = "ca-app-pub-3940256099942544/6300978111";
        private const string TestInterstitialUnitId = "ca-app-pub-3940256099942544/1033173712";
        private const string TestRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";

        [SerializeField] private bool _testMode;
        [SerializeField] private string _bannerUnitId;
        [SerializeField] private string _interstitialUnitId;
        [SerializeField] private string _rewardedAdUnitId;

        public string BannerUnitId => _testMode ? TestBannerUnitId : _bannerUnitId;

        public string InterstitialUnitId => _testMode ? TestInterstitialUnitId : _interstitialUnitId;

        public string RewardedAdUnitId => _testMode ? TestRewardedAdUnitId : _rewardedAdUnitId;
    }
}