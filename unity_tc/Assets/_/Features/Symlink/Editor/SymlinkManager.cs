using System.Diagnostics;
using System.IO;
using UnityEditor;

using Debug = UnityEngine.Debug;

namespace Symlink.Editor
{
    public class SymlinkManager
    {
        private const string SYMLINK_FOLDER_NAME = "symlinks";

        public static void CreateSymlink(string sourcePath, string targetPath)
        {
            if (Directory.Exists(targetPath)) return;
            Debug.unityLogger.logEnabled = false;   // Doesn't work as intended


            string command = $"/c mklink /J \"{targetPath}\" \"{sourcePath}\"";     // Will be obsolete in c# 10

            ProcessStartInfo startInfo = new("cmd.exe", command) { CreateNoWindow = true };

            Process process = Process.Start(startInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Debug.LogError($"<color=red>[SYMLINK]:</color> Failed to create symlink.\nExit code: <color=cyan>{process.ExitCode}</color>");
                return;
            }

            AssetDatabase.Refresh();
            process.Dispose();
            Debug.unityLogger.logEnabled = true;
        }

        public static void RemoveSymlink(string assetFolderPath)
        {
            if (!Directory.Exists(assetFolderPath)) return;

            Directory.Delete(assetFolderPath);
            File.Delete($"{assetFolderPath}.meta");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static string ConvertToAssetsFullPath(string symlinkFolderFullPath)
        {
            string projectPath = Directory.GetCurrentDirectory();
            string projectName = projectPath.Split(@"\")[^1];
            return symlinkFolderFullPath.Replace(SYMLINK_FOLDER_NAME, $@"{projectName}\Assets\_");
        }

        /// <summary>Transfer a file to the symlinks folder.</summary>
        /// <param name="assetPath">Asset path from the asset to move.</param>
        /// <param name="createSymlink">Create symlink from moved folder.</param>
        public static void MoveToSymlinksFile(string assetPath, bool createSymlink = false)
        {
            if (Directory.Exists(assetPath))
            {
                string assetFullPath = Path.GetFullPath(assetPath);
                string projectPath = Directory.GetCurrentDirectory();
                string projectName = projectPath.Split(@"\")[^1];
                string symlinkPath = assetFullPath.Replace($@"{projectName}\Assets\_", SYMLINK_FOLDER_NAME);

                if (Directory.Exists(symlinkPath))
                {
                    Debug.LogError($"<color=red>[SYMLINK]:</color> <color=cyan>{symlinkPath}</color> already exist.");
                    return;
                }

                Directory.Move(assetFullPath, symlinkPath);
                File.Delete($"{assetFullPath}.meta");

                if (createSymlink) CreateSymlink(symlinkPath, assetFullPath);

                AssetDatabase.Refresh();
                return;
            }

            Debug.LogError($"<color=red>[SYMLINK]: The asset path given <color=cyan>{assetPath}</color> does not exist.</color>");
        }
    }
}