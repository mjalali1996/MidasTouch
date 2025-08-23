using System.Collections.Generic;
using System.Linq;
using MidasTouch.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MidasTouch.Editor.Switcher
{
    [InitializeOnLoad]
    public class AdMediationSwitcher : BaseSwitcher
    {
        private const string ADMOB_SYMBOL = "MIDASTOUCH_ADMOB";
        private const string TAPSELL_SYMBOL = "MIDASTOUCH_TAPSELL";

        private const string BaseMediationPackagePath = "Assets/MidasTouch/Editor/Packages/AdMediations";
        protected override string BasePackagePath => BaseMediationPackagePath;

        private static readonly IReadOnlyDictionary<string, string> MediationPackageNames =
            new Dictionary<string, string>()
            {
                { ADMOB_SYMBOL, "Admob" },
                { TAPSELL_SYMBOL, "Tapsell" }
            };

        protected override IReadOnlyDictionary<string, string> PackageNames => MediationPackageNames;

        static AdMediationSwitcher()
        {
            AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
        }

        private static void OnImportPackageCompleted(string packagename)
        {
            var contains = MediationPackageNames.Values.Contains(packagename);
            if (!contains) return;

            GooglePlayServices.PlayServicesResolver.MenuForceResolve();
        }

        [MenuItem("Tools/Ad Mediation Switcher")]
        public static void ShowWindow()
        {
            GetWindow<AdMediationSwitcher>("Ad Mediation Switcher");
        }

        private void OnGUI()
        {
            var currentMediationSymbol = GetCurrentSymbol();
            if (string.IsNullOrEmpty(currentMediationSymbol))
                GUILayout.Label("No Mediation Set");
            else
                GUILayout.Label($"{MediationPackageNames[currentMediationSymbol]} Mediation is active");

            if (GUILayout.Button("Enable AdMob"))
            {
                SwitchTo(ADMOB_SYMBOL);
            }

            if (GUILayout.Button("Enable Tapsell"))
            {
                SwitchTo(TAPSELL_SYMBOL);
            }

            if (GUILayout.Button("Clear All Mediation"))
            {
                ClearAll();
                GooglePlayServices.PlayServicesResolver.MenuForceResolve();
            }
        }
    }
}