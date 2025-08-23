using System.Collections.Generic;
using MidasTouch.Billing.Models;
using UnityEngine;

namespace MidasTouch.Billing.Bazaar
{
    [CreateAssetMenu(menuName = "MidasTouch/Configs/Market/" + nameof(BazaarConfig), fileName = nameof(BazaarConfig))]
    public class BazaarConfig : ScriptableObject
    {
        [SerializeField] private string _rsaKey;
        [SerializeField] private List<Product> _products = new ();
        [SerializeField] private string _webhookAddress;

        public string RsaKey => _rsaKey;
        public IReadOnlyList<Product> Products => _products;
        public string WebhookAddress => _webhookAddress;
    }
}