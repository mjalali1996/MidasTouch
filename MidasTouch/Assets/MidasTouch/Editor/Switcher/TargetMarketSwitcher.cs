using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Purchasing;
using UnityEngine;

namespace MidasTouch.Editor.Switcher
{
    public class TargetMarketSwitcher : BaseSwitcher
    {
        private const string BAZAAR_SYMBOL = "MIDASTOUCH_BAZAAR";
        private const string GOOGLEPLAY_SYMBOL = "MIDASTOUCH_GOOGLEPLAY";
        private const string APPLE_SYMBOL = "MIDASTOUCH_APPLE";

        private const string BaseMarketPackagePath = "Assets/MidasTouch/Editor/Packages/Markets";
        
        const string k_LegacyEnabledSettingName = "Purchasing";
        protected override string BasePackagePath => BaseMarketPackagePath;

        private static readonly List<string> AllSymbols = new()
        {
            BAZAAR_SYMBOL,
            GOOGLEPLAY_SYMBOL,
            APPLE_SYMBOL,
        };

        protected override IReadOnlyList<string> Symbols => AllSymbols;

        private static readonly IReadOnlyDictionary<string, string> MarketSymbolToPackageNames =
            new Dictionary<string, string>()
            {
                { BAZAAR_SYMBOL, "Bazaar" },
            };

        protected override IReadOnlyDictionary<string, string> SymbolToPackageNames => MarketSymbolToPackageNames;

        static TargetMarketSwitcher()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        private static void OnImportPackageCompleted(string packagename)
        {
            var contains = MarketSymbolToPackageNames.Values.Contains(packagename);
            if (!contains) return;

            GooglePlayServices.PlayServicesResolver.MenuForceResolve();
        }

        [MenuItem("Tools/Market Switcher")]
        public static void ShowWindow()
        {
            GetWindow<TargetMarketSwitcher>("Market Switcher");
        }

        private void OnGUI()
        {
            var currentMediationSymbol = GetCurrentSymbol();
            if (string.IsNullOrEmpty(currentMediationSymbol))
                GUILayout.Label("No Market Set");
            else
            {
                var symbolName = currentMediationSymbol.Replace("MIDASTOUCH_", "");
                GUILayout.Label($"{symbolName} Market is active");
            }

            if (GUILayout.Button("Enable Bazaar"))
            {
                SetUnityIapPurchasingEnableSetting(false);
                SwitchTo(BAZAAR_SYMBOL);
            }

            if (GUILayout.Button("Enable GooglePlay"))
            {
                SetUnityIapPurchasingEnableSetting(true);
                SwitchTo(GOOGLEPLAY_SYMBOL);
            }

            if (GUILayout.Button("Enable Apple"))
            {
                SetUnityIapPurchasingEnableSetting(true);
                SwitchTo(APPLE_SYMBOL);
            }

            if (GUILayout.Button("Clear All Markets"))
            {
                SetUnityIapPurchasingEnableSetting(false);
                ClearAll();
                GooglePlayServices.PlayServicesResolver.MenuForceResolve();
            }
        }
        
        static void SetUnityIapPurchasingEnableSetting(bool value)
        {
            PurchasingSettings.enabled = value;
            SetLegacyEnabledSetting(value);
        }

        static void SetLegacyEnabledSetting(bool value)
        {
            var playerSettingsType = Type.GetType("UnityEditor.PlayerSettings,UnityEditor.dll");
            if (playerSettingsType != null)
            {
                var setCloudServiceEnabledMethod = playerSettingsType.GetMethod("SetCloudServiceEnabled", BindingFlags.Static | BindingFlags.NonPublic);
                if (setCloudServiceEnabledMethod != null)
                {
                    setCloudServiceEnabledMethod.Invoke(null, new object[] { k_LegacyEnabledSettingName, value });
                }
            }
        }
    }
}