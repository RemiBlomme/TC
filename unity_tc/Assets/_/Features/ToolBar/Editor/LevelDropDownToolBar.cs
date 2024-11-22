using Level.Data;
using Misc.Data;
using Paps.UnityToolbarExtenderUIToolkit;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace ToolBar.Editor
{
    public static class Levels
    {
        private static bool _overridePlaymode;
        private static LevelDataSO _levelSelected;

        public static void SetOverridePlaymode(bool value)
        {
            _overridePlaymode = value;
            SessionState.SetBool(nameof(_overridePlaymode), _overridePlaymode);
        }

        public static bool GetOverridePlaymode()
        {
            return SessionState.GetBool(nameof(_overridePlaymode), _overridePlaymode);
        }


        public static void SetLevelSelected(LevelDataSO LevelSelected)
        {
            _levelSelected = LevelSelected;
            SessionState.SetString(nameof(_levelSelected), AssetDatabase.GetAssetPath(_levelSelected));
        }

        public static LevelDataSO GetLevelSelected()
        {
            var defaultPath = AssetDatabase.GetAssetPath(_levelSelected);
            var path = SessionState.GetString(nameof(_levelSelected), defaultPath);

            return (LevelDataSO)AssetDatabase.LoadAssetAtPath(path, typeof(LevelDataSO));
        }
    }


    [MainToolbarElement(id: "LevelDropDownToolBar", ToolbarAlign.Left, order: 0)]
    public class LevelDropDownToolBar : DropdownField
    {
        [Serialize] private string _currentField;
        private List<LevelDataSO> _levelDataList = new();

        public void InitializeElement()
        {
            style.minWidth = 150;
            EditorApplication.projectChanged += SetChoices;
            SetChoices();

            this.RegisterValueChangedCallback(OnValueChanged);
            SetValue();
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            _currentField = evt.newValue;

            for (int i = 0; i < _levelDataList.Count; i++)
            {
                if (_currentField == _levelDataList[i].name)
                {
                    Levels.SetLevelSelected(_levelDataList[i]);
                    break;
                }
            }
        }

        private void SetChoices()
        {
            choices = GetAllLevelDataNames();
            if (!choices.Contains(_currentField)) _currentField = null;
        }

        private void SetValue()
        {
            if (choices.Count == 0) return;
            value = _currentField ?? choices[0];
        }

        private List<string> GetAllLevelDataNames()
        {
            List<string> names = new();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(LevelDataSO)}", new[] { Paths.LEVEL_FOLDER_PATH });
            _levelDataList.Clear();

            for (int i = 0; i < guids.Length; i++)
            {
                var current = AssetDatabase.LoadAssetAtPath<LevelDataSO>(AssetDatabase.GUIDToAssetPath(guids[i]));
                _levelDataList.Add(current);
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
            Levels.SetOverridePlaymode(_currentValue);
            value = _currentValue;

            this.RegisterValueChangedCallback(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<bool> evt)
        {
            _currentValue = evt.newValue;
            Levels.SetOverridePlaymode(_currentValue);
        }
    }
}