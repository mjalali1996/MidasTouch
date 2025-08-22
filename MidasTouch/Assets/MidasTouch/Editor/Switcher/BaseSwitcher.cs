using System.Collections.Generic;
using MidasTouch.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MidasTouch.Editor.Switcher
{
    public abstract class BaseSwitcher : EditorWindow
    {
        protected abstract string BasePackagePath { get; }

        protected abstract IReadOnlyDictionary<string, string> PackageNames { get; }


        protected void SwitchTo(string symbol)
        {
            ClearAll();

            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Set the new symbol
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, currentSymbols + ";" + symbol);

            var packagePath = GetPackageFullPath(symbol);
            // Import the package
            AssetDatabase.ImportPackage(packagePath, false);
        }

        protected void ClearAll()
        {
            // PlayServicesResolver.MenuDeleteResolvedLibraries();
            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Remove all custom symbols
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            var newSymbols = currentSymbols;
            string currentSymbol = null;
            foreach (var keyValuePair in PackageNames)
            {
                var mediation = keyValuePair.Key;
                if (currentSymbols.Contains(mediation))
                    currentSymbol = mediation;

                newSymbols = newSymbols.Replace($";{mediation}", "").Replace(mediation, "");
            }

            if (currentSymbol != null)
            {
                var packagePath = GetPackageFullPath(currentSymbol);
                PackageTools.DeleteAllPackageFile(packagePath);
            }

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newSymbols);

            Debug.Log("All Ad Mediation plugins and symbols cleared.");
        }

        private string GetPackageFullPath(string symbol)
        {
            return $"{BasePackagePath}/{PackageNames[symbol]}.unitypackage";
        }

        protected string GetCurrentSymbol()
        {
            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            foreach (var keyValuePair in PackageNames)
            {
                var mediation = keyValuePair.Key;
                if (currentSymbols.Contains(mediation))
                {
                    return mediation;
                }
            }

            return string.Empty;
        }
    }
}