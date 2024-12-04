using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Misc.Data;

namespace AddressableDefinition.Editor
{
    public class AddressableManager
    {
        public static void Scan()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AddressableDefinitionSO)}");

            for (int i = 0; i < guids.Length; i++)
            {
                string parentFolderPath = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[i]));
                string parentFolderName = Path.GetFileName(parentFolderPath);
                string[] childGuids = AssetDatabase.FindAssets("", new[] { parentFolderPath });

                bool alreadyExist = TryGetAddressableAssetGroup(parentFolderName, out AddressableAssetGroup group);
                var newGroup = group ?? NewAddressableAssetGroup(parentFolderName);

                for (int j = 0; j < childGuids.Length; j++)
                {
                    if (AssetDatabase.GetLabels(new GUID(childGuids[j])).Contains(Labels.ADDRESSABLE_IGNORE)) continue;
                    SetAsAddressable($"{childGuids[j]}", newGroup, true);
                }
            }
            Clear();
        }

        public static void Build() => AddressableAssetSettings.BuildPlayerContent();

        public static void Clear()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            for (int i = settings.groups.Count - 1; i > -1; i--)
            {
                if (settings.groups[i].entries.Count == 0 && settings.groups[i] != settings.DefaultGroup)
                {
                    settings.RemoveGroup(settings.groups[i]);
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