using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using ObjectField = UnityEditor.Search.ObjectField;


namespace Misc.Editor
{
    /// <summary>Is using UIToolkit</summary>
    public static class CustomUI
    {
        #region Creators
        [ObsoleteAttribute("Use VisualElement.AddDefaultInspector() extention instead.")]
        public static IMGUIContainer CopyComponentDisplay(UnityEngine.Object obj)
        {
            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(obj);
            return new IMGUIContainer(() => editor.OnInspectorGUI());
        }

        public static T Create<T>(string name = "") where T : VisualElement, new()
        {
            return new T() { name = name };
        }

        public static VisualElement CreateSpace(string name = "", float minWidth = 0, float minHeight = 10)
        {
            return CreateVisualElement(name, minWidth, minHeight);
        }

        public static VisualElement CreateVisualElement(string name = "", float minWidth = 0, float minHeight = 0)
        {
            return new VisualElement() { name = name, style = { minWidth = minWidth, minHeight = minHeight } };
        }

        public static Label CreateLabel(string text = "")
        {
            return new Label() { text = text };
        }

        public static Button CreateButton(string text = "", Action action = null)
        {
            return new Button(action) { text = text };
        }

        public static Toggle CreateToggle(string text = "", bool initialValue = false)
        {
            return new Toggle(text) { value = initialValue };
        }

        public static TextField CreateTextField(string text = "")
        {
            return new TextField(text);
        }

        public static ColorField CreateColorField(string text = "")
        {
            return new ColorField(text);
        }

        public static ObjectField CreateObjectField(Type type, string text = "")
        {
            return new ObjectField(text) { objectType = type };
        }
        #endregion

        #region Copy style
        //private static void ApplyStyle(VisualElement visualElement, IStyle newStyle)
        //{
        //    IStyle style = visualElement.style;

        //    style.alignContent = newStyle.alignContent;
        //    style.alignItems = newStyle.alignItems;
        //    style.alignSelf = newStyle.alignSelf;
        //    style.backgroundColor = newStyle.backgroundColor;
        //    style.backgroundImage = newStyle.backgroundImage;
        //    style.backgroundPositionX = newStyle.backgroundPositionX;
        //    style.backgroundPositionY = newStyle.backgroundPositionY;
        //    style.backgroundRepeat = newStyle.backgroundRepeat;
        //    style.backgroundSize = newStyle.backgroundSize;
        //    style.borderBottomColor = newStyle.borderBottomColor;
        //    style.borderBottomLeftRadius = newStyle.borderBottomLeftRadius;
        //    style.borderBottomRightRadius = newStyle.borderBottomRightRadius;
        //    style.borderBottomWidth = newStyle.borderBottomWidth;
        //    style.borderLeftColor = newStyle.borderLeftColor;
        //    style.borderLeftWidth = newStyle.borderLeftWidth;
        //    style.borderRightColor = newStyle.borderRightColor;
        //    style.borderRightWidth = newStyle.borderRightWidth;
        //    style.borderTopColor = newStyle.borderTopColor;
        //    style.borderTopLeftRadius = newStyle.borderTopLeftRadius;
        //}
        #endregion
    }

    public class VisualElementBuilder
    {
        private VisualElement result = new();


        public VisualElementBuilder(string name = "")
        {
            result = CustomUI.CreateVisualElement(name);
        }

        public VisualElementBuilder Add<T>(T e) where T : VisualElement, new()
        {
            result.Add(e);
            return this;
        }

        public VisualElement Build(string name = "")
        {
            if (name != "") result.AddToClassList(name);
            return result;
        }
    }

    // Test class does not work
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

        private static VisualElement MakeItem() => CustomUI.CreateToggle();

        private static void BindItem(VisualElement element, int index)
        {
            var item = _treeView.GetItemDataForIndex<string>(index);
            (element as Toggle).text = item;
        }
    }

    public static class UIElementsExtensions
    {
        public static VisualElement AddDefaultInspector(this VisualElement visualElement, UnityEditor.Editor editor)
        {
            InspectorElement.FillDefaultInspector(visualElement, editor.serializedObject, editor);
            return visualElement;
        }

        public static Button AddListener(this Button button, EventCallback<ClickEvent, UnityEngine.Object> action, UnityEngine.Object obj)
        {
            button.RegisterCallback(action, obj);
            return button;
        }

        public static Button AddListener(this Button button, Action action)
        {
            button.clicked += action;
            return button;
        }

        public static Toggle AddListener(this Toggle toggle, EventCallback<ChangeEvent<bool>> action)
        {
            toggle.RegisterValueChangedCallback(action);
            return toggle;
        }

        public static TextField AddListener(this TextField textField, EventCallback<ChangeEvent<string>> action)
        {
            textField.RegisterValueChangedCallback(action);
            return textField;
        }

        public static ColorField AddListener(this ColorField colorField, EventCallback<ChangeEvent<Color>> action)
        {
            colorField.RegisterValueChangedCallback(action);
            return colorField;
        }

        public static ObjectField AddListener(this ObjectField objectField, EventCallback<ChangeEvent<UnityEngine.Object>> action)
        {
            objectField.RegisterValueChangedCallback(action);
            return objectField;
        }
    }
}