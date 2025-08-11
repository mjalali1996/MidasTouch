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

            var filePaths = GetPackagePaths(packagePath);
            
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

        public static List<string> GetPackagePaths(string packagePath)
        {
            var filePaths = new List<string>();
            
            if (string.IsNullOrEmpty(packagePath))
            {
                Debug.Log("No file selected. Operation cancelled.");
                return filePaths;
            }

            try
            {
                using var fileStream = new FileStream(packagePath, FileMode.Open, FileAccess.Read);
                using var gzipStream = new GZipInputStream(fileStream);
                using var tarStream = new TarInputStream(gzipStream);
                while (tarStream.GetNextEntry() is { } tarEntry)
                {
                    if (!tarEntry.IsDirectory && tarEntry.Name.EndsWith("pathname"))
                    {
                        string assetPath = new StreamReader(tarStream).ReadLine();

                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            filePaths.Add(assetPath);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to read package: {e.Message}");
                return filePaths;
            }

            return filePaths;
        }
    }
}