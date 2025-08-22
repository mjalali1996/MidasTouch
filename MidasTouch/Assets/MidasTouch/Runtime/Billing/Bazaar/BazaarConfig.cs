using System.Collections.Generic;
using UnityEngine;

namespace MidasTouch.Billing.Bazaar
{
    [CreateAssetMenu(menuName = "MidasTouch/Configs/Market/" + nameof(BazaarConfig), fileName = nameof(BazaarConfig))]
    public class BazaarConfig : ScriptableObject
    {
        [SerializeField] private string _rsaKey;
        [SerializeField] private List<string> _skus = new ();
        [SerializeField] private string _webhookAddress;

        public string RsaKey => _rsaKey;
        public IReadOnlyList<string> SkUs => _skus;
        public string WebhookAddress => _webhookAddress;
    }
}