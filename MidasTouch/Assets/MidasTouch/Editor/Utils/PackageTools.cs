using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using UnityEditor;
using UnityEngine;

namespace MidasTouch.Editor.Utils
{
    public static class PackageTools
    {
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
        
        public static void DeleteAllPackageFile(string packagePath)
        {
            var filePaths = GetPackagePaths(packagePath);

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