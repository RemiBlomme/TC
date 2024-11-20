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
                Debug.LogError("<color=cyan>[SYMLINK]:</color> <color=red>symlink directory not found!</color>");
                return;
            }

            rootVisualElement.Add(VisualTreeBuilder.Get(CreateVisualElement()));

            rootVisualElement.Add(CreateButton("Update", OnUpdateClicked));

            ScrollView scrollView = new();
            DisplaySubDirectories(symlinkDir, scrollView);
            rootVisualElement.Add(scrollView);
        }

        private void OnUpdateClicked()
        {
            for (int i = 0; i < _dirToggles.Count; i++)
            {
                string assetPath = Symlink.ConvertToAssetsFullPath(_symlinkDirInfos[i].FullName);

                if (_dirToggles[i].value) Symlink.CreateSymlink(_symlinkDirInfos[i].FullName, assetPath);
                else Symlink.RemoveSymlink(assetPath);
            }
        }

        private void DisplaySubDirectories(DirectoryInfo symlinkDir, VisualElement root)
        {
            DirectoryInfo[] childDirectories = symlinkDir.GetDirectories();
            for (int i = 0; i < childDirectories.Length; i++)
            {
                _symlinkDirInfos.Add(childDirectories[i]);
                string assetPath = Symlink.ConvertToAssetsFullPath(childDirectories[i].FullName);

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

    public class VisualTreeBuilder
    {
        private static TreeView _treeView;
        private static VisualElement _toAdd;

        public static TreeView Get(VisualElement element)
        {
            var items = new List<TreeViewItemData<string>>(10);
            for (var i = 0; i < items.Count; i++)
            {
                var itemIndex = i * items.Count + i;

                var treeViewSubItemsData = items;
                for (var j = 0; j < items.Count; j++)
                {
                    treeViewSubItemsData.Add(new TreeViewItemData<string>(itemIndex + j + 1, (j + 1).ToString()));
                }

                var treeViewItemData = new TreeViewItemData<string>(itemIndex, (i + 1).ToString(), treeViewSubItemsData);
                items.Add(treeViewItemData);
            }



            _toAdd = element;

            TreeView treeView = new();
            treeView.SetRootItems(items);
            treeView.makeItem = MakeItem;
            treeView.bindItem = BindItem;
            treeView.selectionType = SelectionType.Multiple;
            treeView.Rebuild();

            return treeView;
        }

        private static VisualElement MakeItem() => _toAdd;

        private static void BindItem(VisualElement element, int index)
        {
            var item = _treeView.GetItemDataForIndex<string>(index);
            (element as Label).text = item;
        }
    }
}