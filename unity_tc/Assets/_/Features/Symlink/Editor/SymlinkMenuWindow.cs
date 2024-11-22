using Misc.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using static Misc.Editor.CustomUI;
using TreeView = UnityEngine.UIElements.TreeView;


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
            DirectoryInfo symlinkDir = new DirectoryInfo($"{parentDir.FullName}\\symlinks");

            if (symlinkDir == null)
            {
                Debug.LogError($"<color=red>[SYMLINK]:</color> <color=cyan>{symlinkDir}</color> not found!");
                return;
            }

            //rootVisualElement.Add(VisualTreeMaker.Get(CreateToggle()));

            rootVisualElement.Add(CreateButton("Update", OnUpdateClicked));

            ScrollView scrollView = new();
            DisplaySubDirectories(symlinkDir, scrollView);
            rootVisualElement.Add(scrollView);
        }

        private void OnUpdateClicked()
        {
            for (int i = 0; i < _dirToggles.Count; i++)
            {
                string assetPath = SymlinkManager.ConvertToAssetsFullPath(_symlinkDirInfos[i].FullName);

                if (_dirToggles[i].value) SymlinkManager.CreateSymlink(_symlinkDirInfos[i].FullName, assetPath);
                else SymlinkManager.RemoveSymlink(assetPath);
            }

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

    public class VisualTreeMaker
    {
        private static TreeView _treeView;
        private static VisualElement _toAdd;
        private static int _index;

        public static TreeView Get(VisualElement element)
        {
            _treeView = new TreeView();
            _toAdd = element;

            List<TreeViewItemData<string>> child1Items = new()
            {
                new TreeViewItemData<string>(6 , "0"),
                new TreeViewItemData<string>(7 , "1"),
            };

            List<TreeViewItemData<string>> childItems = new()
            {
                new TreeViewItemData<string>(3 , "0", child1Items),
                new TreeViewItemData<string>(4 , "1"),
                new TreeViewItemData<string>(5 , "2"),
            };

            List<TreeViewItemData<string>> rootItems = new()
            {
                new TreeViewItemData<string>(0 , "0", childItems),
                new TreeViewItemData<string>(1 , "1"),
                new TreeViewItemData<string>(2 , "2"),
            };


            _treeView.SetRootItems(rootItems);
            _treeView.makeItem = MakeItem;
            _treeView.bindItem = BindItem;
            _treeView.Rebuild();

            return _treeView;
        }

        private TreeViewItemData<string> GetNewItem(string text)
        {
            return new TreeViewItemData<string>(_index++, text);
        }

        private static VisualElement MakeItem() => CreateToggle();

        private static void BindItem(VisualElement element, int index)
        {
            var item = _treeView.GetItemDataForIndex<string>(index);
            (element as Toggle).text = item;
        }
    }
}