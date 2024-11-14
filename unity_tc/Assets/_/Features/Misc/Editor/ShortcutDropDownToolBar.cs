using Paps.UnityToolbarExtenderUIToolkit;
using System;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.UIElements;

namespace Misc.Editor
{
    //[MainToolbarElement(id: "RightToolBar", ToolbarAlign.Right, order: 0)]
    //public class RightToolBar : VisualElement
    //{
    //    private ShortcutDropDownToolBar _dropdownShortcut;
    //    private CreateFeatureToolBar _button;

    //    private VisualElement _lastVisualElement;


    //    public void InitializeElement()
    //    {
    //        style.minWidth = 200;
    //        style.flexDirection = FlexDirection.Row;

    //        _dropdownShortcut = new();
    //        _dropdownShortcut.InitializeElement();
    //        _dropdownShortcut.RegisterValueChangedCallback((evt) => ChangeElements(evt.newValue));
    //        Add(_dropdownShortcut);

    //        _button = new();
    //        _button.InitializeElement();
    //        Add(_button);
    //    }

    //    private VisualElement ChangeElements(string dropDownChoice)
    //    {
    //        switch (dropDownChoice)
    //        {
    //            case "Features":
    //                AddElement(_button);
    //                return _button;

    //            default:
    //                AddElement(null);
    //                return null;
    //        }
    //    }

    //    private void AddElement(VisualElement visualElement)
    //    {
    //        if (_lastVisualElement != null) _lastVisualElement.style.display = DisplayStyle.None;
    //        visualElement.style.display = DisplayStyle.Flex;
    //        _lastVisualElement = visualElement;
    //    }
    //}

    public static class ToolBarShortcuts
    {
        public const string ADDRESSABLE = "Addressable";
        public const string FEATURE = "Feature";
        public const string LEVEL = "Level";

        public static readonly string[] ARRAY = new[] { ADDRESSABLE, FEATURE, LEVEL };
    }


    [MainToolbarElement(id: "ShortcutDropDownToolBar", ToolbarAlign.Right, order: 0)]
    public class ShortcutDropDownToolBar : DropdownField
    {
        public static event Action<string> OnValueChange;

        [Serialize] private string _currentValue;

        public void InitializeElement()
        {
            label = "Shortcuts";
            choices = new(ToolBarShortcuts.ARRAY);

            value = _currentValue ?? choices[0];
            this.RegisterValueChangedCallback(OnRegisterValueChanged);
        }

        private void OnRegisterValueChanged(ChangeEvent<string> evt)
        {
            _currentValue = evt.newValue;
            OnValueChange?.Invoke(_currentValue);
        }
    }


    [MainToolbarElement(id: "CreateFeatureButtonToolBar", ToolbarAlign.Right, order: 1)]
    public class CreateFeatureButtonToolBar : Button
    {
        [Serialize] private DisplayStyle _currentDisplayStyle;

        public void InitializeElement()
        {
            text = "Create";
            clicked += () => WindowFeatureCreator.ShowWindow();

            style.display = _currentDisplayStyle;
            ShortcutDropDownToolBar.OnValueChange += OnShortcutDropDownValueChange;
        }

        private void OnShortcutDropDownValueChange(string value)
        {
            _currentDisplayStyle = value == ToolBarShortcuts.FEATURE ? DisplayStyle.Flex : DisplayStyle.None;
            style.display = _currentDisplayStyle;
        }
    }


    [MainToolbarElement(id: "CreateLevelButtonToolBar", ToolbarAlign.Right, order: 1)]
    public class CreateLevelButtonToolBar : Button
    {
        [Serialize] private DisplayStyle _currentDisplayStyle;

        public void InitializeElement()
        {
            text = "Create";
            clicked += () => WindowLevelCreator.ShowWindow();

            style.display = _currentDisplayStyle;
            ShortcutDropDownToolBar.OnValueChange += OnShortcutDropDownValueChange;
        }

        private void OnShortcutDropDownValueChange(string value)
        {
            _currentDisplayStyle = value == ToolBarShortcuts.LEVEL ? DisplayStyle.Flex : DisplayStyle.None;
            style.display = _currentDisplayStyle;
        }
    }


    [MainToolbarElement(id: "BuildAddressableButtonToolBar", ToolbarAlign.Right, order: 1)]
    public class BuildAddressableButtonToolBar : Button
    {
        [Serialize] private DisplayStyle _currentDisplayStyle;

        public void InitializeElement()
        {
            text = "Build";
            clicked += () => clicked += () => AddressableAssetSettings.BuildPlayerContent();

            style.display = _currentDisplayStyle;
            ShortcutDropDownToolBar.OnValueChange += OnShortcutDropDownValueChange;
        }

        private void OnShortcutDropDownValueChange(string value)
        {
            _currentDisplayStyle = value == ToolBarShortcuts.ADDRESSABLE ? DisplayStyle.Flex : DisplayStyle.None;
            style.display = _currentDisplayStyle;
        }
    }
}