using Misc.Runtime;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

using Debug = UnityEngine.Debug;

namespace Symlink.Editor
{
    public class Symlink : MonoBehaviour
    {
        private const string SYMLINK_FOLDER = "symlinks";

        public static string ConvertToAssetsFullPath(string symlinkFloderPath)
        {
            int index = symlinkFloderPath.LastIndexOf(SYMLINK_FOLDER, StringComparison.OrdinalIgnoreCase);
            index += SYMLINK_FOLDER.Length;
            string path = $"{symlinkFloderPath[index..].TrimStart(Path.DirectorySeparatorChar)}";

            return $"{Directory.GetCurrentDirectory()}/{Paths.ROOT_FOLDER}/{path}";
        }

        public static void CreateSymlink(string sourcePath, string targetPath)
        {
            if (Directory.Exists(targetPath)) return;

            string command = $"/c mklink /J \"{targetPath}\" \"{sourcePath}\"";

            ProcessStartInfo startInfo = new("cmd.exe", command) { CreateNoWindow = true };
            
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            process.Dispose();

            if (process.ExitCode == 0)
            {
                Debug.Log($"Symlink created: <color=cyan>{sourcePath}</color> -> <color=cyan>{targetPath}</color>");
            }
            else
            {
                Debug.LogError($"<color=cyan>[SYMLINK]:</color> <color=red>Failed to create symlink. Exit code:</color> <color=cyan>{process.ExitCode}</color>");
                return;
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static void RemoveSymlink(string assetFolderPath)
        {
            if (!Directory.Exists(assetFolderPath)) return;

            Directory.Delete(assetFolderPath);
            File.Delete($"{assetFolderPath}.meta");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}