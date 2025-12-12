using System;
using System.Collections.Generic;
using MidasTouch.Billing.Models;
using UnityEngine;

namespace MidasTouch.Billing
{
    [CreateAssetMenu(menuName = "MidasTouch/Configs/Market/" + nameof(BillingConfig), fileName = nameof(BillingConfig))]
    public class BillingConfig : ScriptableObject
    {
        [SerializeField] private BazaarConfig _bazaarConfig;
        public BazaarConfig BazaarConfig => _bazaarConfig;

        [SerializeField] private MyketConfig _myketConfig;
        public MyketConfig MyketConfig => _myketConfig;

        [SerializeField] private UnityIAPConfig _unityIAPConfig;
        public UnityIAPConfig UnityIAPConfig => _unityIAPConfig;

        [SerializeField] private List<Product> _products = new();
        public IReadOnlyList<Product> Products => _products;

        [SerializeField] private string _webhookAddress;
        public string WebhookAddress => _webhookAddress;
    }

    [Serializable]
    public class BazaarConfig
    {
        [SerializeField] private string _rsaKey;
        public string RsaKey => _rsaKey;
    }


    [Serializable]
    public class MyketConfig
    {
        [SerializeField] private string _rsaKey;
        public string RsaKey => _rsaKey;
    }


    [Serializable]
    public class UnityIAPConfig
    {
        [SerializeField] private string _googlePublicKey;
        public string GooglePublicKey => _googlePublicKey;

        [SerializeField] private string _googleBundleId;
        public string GoogleBundleId => _googleBundleId;

        [SerializeField] private string _rootCert;
        public string RootCert => _rootCert;

        [SerializeField] private string _appleBundleId;
        public string AppleBundleId => _appleBundleId;
    }
}