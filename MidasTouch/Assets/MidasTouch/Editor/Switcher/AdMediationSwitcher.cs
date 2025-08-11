using System;
using System.Collections.Generic;
using System.IO;
using MidasTouch.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace MidasTouch.Editor.Switcher
{
    public class AdMediationSwitcher : EditorWindow
    {
        private const string ADMOB_SYMBOL = "USE_ADMOB";
        private const string TAPSELL_SYMBOL = "USE_TAPSELL";
        private const string BaseMediationPath = "Assets/MidasTouch/Editor/AdMediation";
        
        private static readonly Dictionary<string, string> MediationPaths = new()
        {
            { ADMOB_SYMBOL, $"{BaseMediationPath}/Admob.unitypackage" },
            { TAPSELL_SYMBOL, $"{BaseMediationPath}/Tapsell.unitypackage" }
        };
        
        [MenuItem("Tools/Ad Mediation Switcher")]
        public static void ShowWindow()
        {
            GetWindow<AdMediationSwitcher>("Ad Mediation Switcher");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Enable AdMob"))
            {
                SwitchToMediation(ADMOB_SYMBOL, MediationPaths[ADMOB_SYMBOL]);
            }

            if (GUILayout.Button("Enable Tapsell"))
            {
                SwitchToMediation(TAPSELL_SYMBOL, MediationPaths[TAPSELL_SYMBOL]);
            }

            if (GUILayout.Button("Clear All Mediation"))
            {
                ClearAllMediation();
            }
        }

        private void SwitchToMediation(string symbol, string packagePath)
        {
            ClearAllMediation();

            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            
            // Set the new symbol
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, currentSymbols + ";" + symbol);

            // Import the package
            AssetDatabase.ImportPackage(packagePath, false);
        }

        private void ClearAllMediation()
        {
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            
            // Remove all custom symbols
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
            var newSymbols = currentSymbols;
            string currentMediation = ADMOB_SYMBOL;
            foreach (var keyValuePair in MediationPaths)
            {
                var mediation = keyValuePair.Key;
                if (currentSymbols.Contains(mediation)) 
                    currentMediation = mediation;

                newSymbols = newSymbols.Replace($";{mediation}", "").Replace(mediation, "");

            }

            if (currentMediation != null) 
                DeleteAllMediationFile(MediationPaths[currentMediation]);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newSymbols);
            
            Debug.Log("All Ad Mediation plugins and symbols cleared.");
        }

        private void DeleteAllMediationFile(string packagePath)
        {
            var filePaths = PackagePathExtractor.GetPackagePaths(packagePath);
            
            var parentDirectories = new HashSet<string>();

            // Pass 1: Delete all files and folders from the extracted paths
            foreach (var path in filePaths)
            {
                // Get the parent directory of the path and add it to our list for later cleanup

                var fullPath = Path.Combine(Application.dataPath, path.Substring("Assets/".Length));
                
                var parentDir = Path.GetDirectoryName(fullPath)?.Replace('\\', '/');
                if (!string.IsNullOrEmpty(parentDir))
                {
                    parentDirectories.Add(parentDir);
                }
        
                if (File.Exists(fullPath) || Directory.Exists(fullPath))
                {
                    // Use FileUtil.DeleteFileOrDirectory, which correctly handles assets
                    FileUtil.DeleteFileOrDirectory(path);
                    var metaFile = path + ".meta";
                    if (File.Exists(metaFile)) 
                        FileUtil.DeleteFileOrDirectory(metaFile);
                    Debug.Log($"Deleted: {path}");
                }
                else
                {
                    Debug.LogWarning($"Skipped (not found): {path}");
                }
            }
    
            // Pass 2: Check and delete any parent directories that are now empty
            Debug.Log("--- Starting empty directory cleanup ---");
    
            // Sort the directories in reverse alphabetical order to process deepest directories first
            var sortedDirectories = new List<string>(parentDirectories);
            sortedDirectories.Sort((a, b) => String.CompareOrdinal(b, a));

            foreach (var dir in sortedDirectories)
            {
                // Check if the directory exists and if it's empty
                // GetFileSystemEntries returns all files and subdirectories
                if (Directory.Exists(dir) && Directory.GetFileSystemEntries(dir).Length == 0)
                {
                    FileUtil.DeleteFileOrDirectory(dir);
                    
                    var metaFile = dir + ".meta";
                    if (File.Exists(metaFile)) FileUtil.DeleteFileOrDirectory(metaFile);
                    
                    Debug.Log($"Deleted empty directory: {dir}");
                }
            }

            // Finalize by refreshing the AssetDatabase to reflect all changes
            AssetDatabase.Refresh();
            Debug.Log("--- Deletion complete and AssetDatabase refreshed ---");
        }
    }
}