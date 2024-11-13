using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.Search.ObjectField;

namespace CustomWindowCreator.Editor
{
    public abstract class CustomWindow : EditorWindow
    {
        [SerializeField] protected StyleSheet _styleSheet;
        protected VisualElement _root;


        private void CreateGUI()
        {
            _root = rootVisualElement;
            if (_styleSheet) _root.styleSheets.Add(_styleSheet);
            Create();
        }

        protected abstract void Create();

        #region Creators
        public IMGUIContainer CopyComponentDisplay(UnityEngine.Object obj)
        {
            UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor(obj);
            return new IMGUIContainer(() => editor.OnInspectorGUI());
        }

        public T Create<T>(string style = "") where T : VisualElement, new()
        {
            var current = new T();
            if (style != "") current.AddToClassList(style);
            return current;
        }

        public VisualElement CreateVisualElement(string text = "", string style = "")
        {
            var current = new VisualElement() { name = text };
            current.AddToClassList(style);
            return current;
        }

        public Label CreateLabel(string text = "", string style = "")
        {
            var current = new Label(text);
            current.AddToClassList(style);
            return current;
        }

        public Button CreateButton(string text = "", string style = "")
        {
            var current = new Button() { text = text };
            current.AddToClassList(style);
            return current;
        }

        public Toggle CreateToggle(bool link, string text = "", string style = "")
        {
            var current = new Toggle(text) { value = link };
            current.AddToClassList(style);
            return current;
        }

        public TextField CreateTextField(string text = "", string style = "")
        {
            var current = new TextField(text);
            current.AddToClassList(style);
            return current;
        }

        public ColorField CreateColorField(string text = "", string style = "")
        {
            var current = new ColorField(text);
            current.AddToClassList(style);
            return current;
        }

        public ObjectField CreateObjectField(Type type, string text = "", string style = "")
        {
            var current = new ObjectField(text) { objectType = type };
            current.AddToClassList(style);
            return current;
        }
        #endregion
    }

    public class VisualElementBuilder
    {
        private VisualElement result = new();

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

    #region Extention Method

    public static class CustomWindowExtensions
    {
        public static Button AddListener(this Button button, EventCallback<ClickEvent, UnityEngine.Object> action, UnityEngine.Object obj)
        {
            button.RegisterCallback<ClickEvent, UnityEngine.Object>(action, obj);
            return button;
        }

        public static Button AddListener(this Button button, Action action)
        {
            button.clicked += action;
            return button;
        }

        private static void Button_clicked()
        {
            throw new NotImplementedException();
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

    #endregion
}