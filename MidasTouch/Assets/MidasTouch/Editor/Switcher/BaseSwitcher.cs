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

        protected abstract IReadOnlyList<string> Symbols { get; }
        protected abstract IReadOnlyDictionary<string, string> SymbolToPackageNames { get; }


        protected void SwitchTo(string symbol)
        {
            ClearAll();

            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Set the new symbol
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, currentSymbols + ";" + symbol);

            TryImportPackage(symbol);
        }

        protected void ClearAll()
        {
            var currentSymbol = GetCurrentSymbol();
            if (!string.IsNullOrEmpty(currentSymbol)) 
                TryDeletePackage(currentSymbol);

            ClearSymbols();
        }

        private void ClearSymbols()
        {
            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            // Remove all custom symbols
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            var newSymbols = currentSymbols;
            foreach (var symbol in Symbols)
            {
                newSymbols = newSymbols.Replace($";{symbol}", "").Replace(symbol, "");
            }

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newSymbols);

            Debug.Log("Symbols cleared.");
        }

        private void TryImportPackage(string symbol)
        {
            if (!SymbolToPackageNames.ContainsKey(symbol)) return;
            var packagePath = GetPackageFullPath(symbol);
            // Import the package
            AssetDatabase.ImportPackage(packagePath, false);
        }

        private void TryDeletePackage(string symbol)
        {
            if (!SymbolToPackageNames.ContainsKey(symbol)) return;
            var packagePath = GetPackageFullPath(symbol);
            PackageTools.DeleteAllPackageFile(packagePath);
        }

        private string GetPackageFullPath(string symbol)
        {
            return $"{BasePackagePath}/{SymbolToPackageNames[symbol]}.unitypackage";
        }

        protected string GetCurrentSymbol()
        {
            var namedBuildTarget =
                NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            foreach (var symbol in Symbols)
            {
                if (currentSymbols.Contains(symbol))
                {
                    return symbol;
                }
            }

            return string.Empty;
        }
    }
}