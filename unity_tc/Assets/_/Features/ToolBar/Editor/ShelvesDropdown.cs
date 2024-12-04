using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using Paps.UnityToolbarExtenderUIToolkit;
using Misc.Editor;
using Level.Editor;
using AddressableDefinition.Editor;

namespace ToolBar.Editor
{
    public static class Shelves
    {
        public const string ADDRESSABLE = "Addressable";
        public const string FEATURE = "Feature";
        public const string LEVEL = "Level";

        public static readonly string[] CHOICES = new[] { ADDRESSABLE, FEATURE, LEVEL };

        private static event Action<string> Changed;


        public static void SubscribeChanged(Action<string> action) => Changed += action;
        public static void UnsubscribeChanged(Action<string> action) => Changed -= action;
        public static void RaiseChanged(string value) => Changed?.Invoke(value);
    }


    [MainToolbarElement(id: nameof(ShelvesDropdown), ToolbarAlign.Right, order: 0)]
    public class ShelvesDropdown : DropdownField
    {
        [Serialize] private string _currentValue;

        public async void InitializeElement()
        {
            style.minWidth = 150;
            label = "";
            choices = new(Shelves.CHOICES);

            if (choices.Count == 0) return;
            value = _currentValue ?? choices[0];
            this.RegisterValueChangedCallback(OnRegisterValueChanged);
            await Task.Delay(100);
            Shelves.RaiseChanged(value);
        }

        private void OnRegisterValueChanged(ChangeEvent<string> evt)
        {
            _currentValue = evt.newValue;
            Shelves.RaiseChanged(_currentValue);
        }
    }

    public abstract class Shelf : Button
    {
        [Serialize] private DisplayStyle _currentDisplayStyle;
        protected string _shortcut;

        public abstract void Initialize();
        protected void InitializeElement()
        {
            Initialize();

            style.display = DisplayStyle.None;
            Shelves.SubscribeChanged(OnShelvesChanged);
        }

        private void OnShelvesChanged(string value)
        {
            _currentDisplayStyle = value == _shortcut ? DisplayStyle.Flex : DisplayStyle.None;
            style.display = _currentDisplayStyle;
        }
    }


    [MainToolbarElement(id: nameof(CreateFeatureShelf), ToolbarAlign.Right, order: 1)]
    public class CreateFeatureShelf : Shelf
    {
        public override void Initialize()
        {
            _shortcut = Shelves.FEATURE;

            text = "Create";
            clicked += WindowFeatureCreator.ShowWindow;
        }
    }


    [MainToolbarElement(id: nameof(CreateLevelShelf), ToolbarAlign.Right, order: 0)]
    public class CreateLevelShelf : Shelf
    {
        public override void Initialize()
        {
            _shortcut = Shelves.LEVEL;

            text = "Create";
            clicked += WindowLevelCreator.ShowWindow;
        }
    }


    [MainToolbarElement(id: nameof(ShowAddressabeGroupShelf), ToolbarAlign.Right, order: 1)]
    public class ShowAddressabeGroupShelf : Shelf
    {
        public override void Initialize()
        {
            _shortcut = Shelves.ADDRESSABLE;

            text = "Groups";
            clicked += AddressableManager.OpenGroupWindow;
        }
    }

    [MainToolbarElement(id: nameof(ScanAddressableShelf), ToolbarAlign.Right, order: 2)]
    public class ScanAddressableShelf : Shelf
    {
        public override void Initialize()
        {
            _shortcut = Shelves.ADDRESSABLE;

            text = "Scan";
            clicked += AddressableManager.Scan;
        }
    }

    [MainToolbarElement(id: nameof(BuildAddressableShelf), ToolbarAlign.Right, order: 3)]
    public class BuildAddressableShelf : Shelf
    {
        public override void Initialize()
        {
            _shortcut = Shelves.ADDRESSABLE;

            text = "Build";
            clicked += AddressableManager.Build;
        }
    }

    [MainToolbarElement(id: nameof(ClearAddressableShelf), ToolbarAlign.Right, order: 4)]
    public class ClearAddressableShelf : Shelf
    {
        public override void Initialize()
        {
            _shortcut = Shelves.ADDRESSABLE;

            text = "Clear";
            clicked += AddressableManager.Clear;
        }
    }
}