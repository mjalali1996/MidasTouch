using UnityEngine;

namespace MidasTouch.AD.Tapsell
{
    [CreateAssetMenu(menuName = "MidasTouch/Configs/Ad" + nameof(TapsellConfig), fileName = nameof(TapsellConfig))]
    public class TapsellConfig : ScriptableObject
    {
        private const string TestAppId = "alsoatsrtrotpqacegkehkaiieckldhrgsbspqtgqnbrrfccrtbdomgjtahflchkqtqosa";
        private const string TestBannerZoneId = "5cfaaa30e8d17f0001ffb294";
        private const string TestInterstitialZoneId = "5cfaa942e8d17f0001ffb292";
        private const string TestRewardedZoneId = "5cfaa802e8d17f0001ffb28e";

        [SerializeField] private bool _testMode;
        [SerializeField] private string _appId;
        [SerializeField] private string _bannerZoneId;
        [SerializeField] private string _interstitialZoneId;
        [SerializeField] private string _rewardedZoneId;

        public string AppId => _testMode ? TestAppId : _appId;
        public string BannerZoneId => _testMode ? TestBannerZoneId : _bannerZoneId;
        public string InterstitialZoneId => _testMode ? TestInterstitialZoneId : _interstitialZoneId;
        public string RewardedZoneId => _testMode ? TestRewardedZoneId : _rewardedZoneId;
    }
}