using Level.Data;
using Misc.Runtime;
using Paps.UnityToolbarExtenderUIToolkit;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Misc.Editor
{
    [MainToolbarElement(id: "LevelDropDownToolBar", ToolbarAlign.Left, order: 0)]
    public class LevelDropDownToolBar : DropdownField
    {
        [Serialize] private string _currentField;

        public void InitializeElement()
        {
            style.minWidth = 150;
            EditorApplication.projectChanged += SetChoices;
            SetChoices();

            this.RegisterValueChangedCallback((evt) => _currentField = evt.newValue);
        }


        private void SetChoices()
        {
            choices = GetAllLevelDataNames();
            if (!choices.Contains(_currentField)) _currentField = null;
            value = _currentField ?? choices[0];
        }

        private List<string> GetAllLevelDataNames()
        {
            List<string> names = new();

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(LevelDataSO)}", new[] { Paths.LEVEL_FOLDER_PATH });

            for (int i = 0; i < guids.Length; i++)
            {
                var current = AssetDatabase.LoadAssetAtPath<LevelDataSO>(AssetDatabase.GUIDToAssetPath(guids[i]));
                names.Add(current.name);
            }

            return names;
        }
    }

    [MainToolbarElement(id: "TogglePlayModeOverrideToolBar", ToolbarAlign.Left, order: 1)]
    public class TogglePlayModeOverrideToolBar : Toggle
    {
        [Serialize] private bool _currentValue;

        public void InitializeElement()
        {
            label = "Override Playmode";
            value = _currentValue;

            this.RegisterValueChangedCallback((evt) => _currentValue = evt.newValue);
        }
    }
}