using System.IO;
using UnityEditor;
using UnityEngine;

namespace Misc.Editor
{
    [InitializeOnLoad]
    public class ProjectExtensionDrawer : MonoBehaviour
    {
        private const int SYMLINK_DOT_WIDTH = 15;
        private static readonly Color SYMLINK_DOT_COLOR = new Color(0f, 0.75f, 1f);


        static ProjectExtensionDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect rect)
        {
            MarkOnSymlinkFolder(guid, rect);
        }

        private static void MarkOnSymlinkFolder(string guid, Rect rect)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return;
            if (!File.GetAttributes(assetPath).HasFlag(FileAttributes.ReparsePoint)) return;

            var previousColor = GUI.contentColor;

            GUI.contentColor = SYMLINK_DOT_COLOR;
            rect.x += rect.width - SYMLINK_DOT_WIDTH;
            rect.width = SYMLINK_DOT_WIDTH;

            GUI.Label(rect, "\u2022");  // It's a dot

            GUI.contentColor = previousColor;
        }
    }
}