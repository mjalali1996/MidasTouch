using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using UnityEditor;
using UnityEngine;

namespace MidasTouch.Editor.Utils
{
    public class PackagePathExtractor
    {
        [MenuItem("Tools/Extract Package Paths")]
        private static void ExtractPathsFromPackage()
        {
            // Let the user select the .unitypackage file
            var packagePath = EditorUtility.OpenFilePanel("Select .unitypackage file", "", "unitypackage");

            var filePaths = PackageTools.GetPackagePaths(packagePath);

            // Print the extracted paths to the console
            if (filePaths.Count > 0)
            {
                Debug.Log("--- Extracted file paths from " + Path.GetFileName(packagePath) + " ---");
                foreach (string path in filePaths)
                {
                    Debug.Log(path);
                }

                Debug.Log("--- Extraction complete ---");
            }
            else
            {
                Debug.Log("No file paths found in the package.");
            }
        }
    }
}