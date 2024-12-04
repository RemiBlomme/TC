using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Misc.Data;
using UnityEditor.AddressableAssets.GUI;
using AddressableDefinition.Data;

namespace AddressableDefinition.Editor
{
    public class AddressableManager
    {
        public static void Scan()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AddressableDefinitionSO)}");

            for (int i = 0; i < guids.Length; i++)
            {
                string definitionPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                string parentFolderPath = Path.GetDirectoryName(definitionPath);
                //string parentFolderName = Path.GetFileName(parentFolderPath);
                string[] childGuids = AssetDatabase.FindAssets("", new[] { parentFolderPath });

                string groupName = AssetDatabase.LoadAssetAtPath<AddressableDefinitionSO>(definitionPath).name;

                //bool alreadyExist = TryGetAddressableAssetGroup(parentFolderName, out AddressableAssetGroup group);
                //var newGroup = group ?? NewAddressableAssetGroup(parentFolderName);

                var newGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName) ?? null;
                AddressableAssetSettingsDefaultObject.Settings.CreateGroup(groupName, false, false, true, null);
                ProcessFolderRecurcively(parentFolderPath, newGroup);

                //for (int j = 0; j < childGuids.Length; j++)
                //{
                //    if (AssetDatabase.GetLabels(new GUID(childGuids[j])).Contains(Labels.ADDRESSABLE_IGNORE)) continue;
                //    SetAsAddressable($"{childGuids[j]}", newGroup, true);
                //}
            }
            Clear();
        }

        private static void ProcessFolderRecurcively(string folderPath, AddressableAssetGroup group)
        {
            AddFilesToGroup(Directory.GetFiles(folderPath), group);
            string[] subFolders = Directory.GetDirectories(folderPath);

            for (int i = 0; i < subFolders.Length; i++)
            {
                var subFolder = subFolders[i];
                string[] subFolderFiles = Directory.GetFiles(subFolder);
                bool hasAddressableDefinition = subFolderFiles.Any(subFile => AssetDatabase.LoadAssetAtPath<AddressableDefinitionSO>(subFile));
                if (hasAddressableDefinition) continue;

                AddFilesToGroup(subFolderFiles, group);
                ProcessFolderRecurcively(subFolder, group);
            }
        }

        private static void AddFilesToGroup(string[] filesPaths, AddressableAssetGroup group)
        {
            string[] files = filesPaths.Where(file => !file.EndsWith(".meta")).ToArray();

            for (int i = 0; i < files.Length; i++)
            {
                GUID guid = AssetDatabase.GUIDFromAssetPath(files[i]);
                if (AssetDatabase.GetLabels(guid).Contains(Labels.ADDRESSABLE_IGNORE)) continue;

                AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry($"{guid}", group);
            }
        }

        public static void OpenGroupWindow() => AddressableExposed.GetGroupWindow();

        public static void Build() => AddressableAssetSettings.BuildPlayerContent();

        public static void Clear()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group;

            for (int i = settings.groups.Count - 1; i > -1; i--)
            {
                group = settings.groups[i];
                if (group.entries.Count == 0 && !group.IsDefaultGroup())
                {
                    settings.RemoveGroup(group);
                }
            }
        }

        public static AssetReference SetAsAddressable(string guid, AddressableAssetGroup group = null, bool refreshDataBase = false)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (group is null) group = settings.DefaultGroup;

            var entry = settings.CreateOrMoveEntry(guid, group);
            if (refreshDataBase) AssetDatabase.SaveAssetIfDirty(new GUID(entry.guid));

            return new(entry.guid);
        }

        public static bool TryGetAddressableAssetGroup(string groupName, out AddressableAssetGroup group)
        {
            group = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
            return group;
        }

        public static AddressableAssetGroup NewAddressableAssetGroup(string groupName)
        {
            if (TryGetAddressableAssetGroup(groupName, out AddressableAssetGroup group))
            {
                Debug.Log($"<color=red>[ADDRESSABLE]:</color> The group <color=cyan>{groupName}</color> that you are trying to create already exist.");
                return null;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            return settings.CreateGroup(groupName, false, false, true, null);
        }
    }
}