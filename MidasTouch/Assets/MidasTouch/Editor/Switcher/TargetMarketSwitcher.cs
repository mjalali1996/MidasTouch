using System.Collections.Generic;
using System.Linq;
using MidasTouch.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MidasTouch.Editor.Switcher
{
    public class TargetMarketSwitcher : BaseSwitcher
    {
        private const string BAZAAR_SYMBOL = "MIDASTOUCH_BAZAAR";
        private const string GOOGLEPLAY_SYMBOL = "MIDASTOUCH_GOOGLEPLAY";

        private const string BaseMarketPackagePath = "Assets/MidasTouch/Editor/Packages/Markets";
        protected override string BasePackagePath => BaseMarketPackagePath;

        private static readonly IReadOnlyDictionary<string, string> MarketPackageNames =
            new Dictionary<string, string>()
            {
                { BAZAAR_SYMBOL, "Bazaar" },
                { GOOGLEPLAY_SYMBOL, "GooglePlay" }
            };
        
        protected override IReadOnlyDictionary<string, string> PackageNames => MarketPackageNames;

        static TargetMarketSwitcher()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        private static void OnImportPackageCompleted(string packagename)
        {
            var contains = MarketPackageNames.Values.Contains(packagename);
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
                GUILayout.Label($"{MarketPackageNames[currentMediationSymbol]} Market is active");

            if (GUILayout.Button("Enable Bazaar"))
            {
                SwitchTo(BAZAAR_SYMBOL);
            }

            GUI.enabled = false;
            if (GUILayout.Button("Enable GooglePlay"))
            {
                SwitchTo(GOOGLEPLAY_SYMBOL);
            }
            GUI.enabled = true;

            if (GUILayout.Button("Clear All Markets"))
            {
                ClearAll();
                GooglePlayServices.PlayServicesResolver.MenuForceResolve();
            }
        }

    }
}