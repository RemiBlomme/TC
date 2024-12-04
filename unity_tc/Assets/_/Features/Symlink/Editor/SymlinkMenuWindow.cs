using AddressableDefinition.Editor;
using Misc.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using static Misc.Editor.CustomUI;

namespace Symlink.Editor
{
    public class SymlinkMenuWindow : EditorWindow
    {
        private List<DirectoryInfo> _symlinkDirInfos = new();
        private List<Toggle> _dirToggles = new();


        [MenuItem("Symlink/Directories")]
        public static void ShowWindow()
        {
            GetWindow<SymlinkMenuWindow>("Symlinks");
        }

        private void CreateGUI()
        {
            string dirPath = Directory.GetCurrentDirectory();
            DirectoryInfo parentDir = Directory.GetParent(dirPath);
            DirectoryInfo symlinkDir = new($"{parentDir.FullName}\\symlinks");

            if (symlinkDir == null)
            {
                Debug.LogError($"<color=red>[SYMLINK]:</color> <color=cyan>{symlinkDir}</color> not found!");
                return;
            }

            //rootVisualElement.Add(VisualTreeMaker.Get(CreateToggle()));   // Started to create a VisualTreeElement to replace the current visual.

            ScrollView scrollView = new();
            DisplaySubDirectories(symlinkDir, scrollView);
            rootVisualElement.Add(scrollView);

            rootVisualElement.Add(CreateButton("Update", OnUpdateClicked));
        }

        private void OnUpdateClicked()
        {
            for (int i = 0; i < _dirToggles.Count; i++)
            {
                string assetPath = SymlinkManager.ConvertToAssetsFullPath(_symlinkDirInfos[i].FullName);

                if (_dirToggles[i].value) SymlinkManager.CreateSymlink(_symlinkDirInfos[i].FullName, assetPath);
                else SymlinkManager.RemoveSymlink(assetPath);
            }

            Close();
            AddressableManager.Scan();
        }

        private void DisplaySubDirectories(DirectoryInfo symlinkDir, VisualElement root)
        {
            DirectoryInfo[] childDirectories = symlinkDir.GetDirectories();
            for (int i = 0; i < childDirectories.Length; i++)
            {
                _symlinkDirInfos.Add(childDirectories[i]);
                string assetPath = SymlinkManager.ConvertToAssetsFullPath(childDirectories[i].FullName);

                VisualElement header;
                Toggle toggle;

                root.Add(header = new VisualElementBuilder()
                    .Add(toggle = CreateToggle("", Directory.Exists(assetPath)))
                    .Add(CreateLabel(childDirectories[i].Name))
                    .Build());

                _dirToggles.Add(toggle);

                header.style.flexDirection = FlexDirection.Row;

                VisualElement subDirectory = new();
                subDirectory.style.marginLeft = 15;
                root.Add(subDirectory);

                DisplaySubDirectories(childDirectories[i], subDirectory);
            }
        }
    }
}