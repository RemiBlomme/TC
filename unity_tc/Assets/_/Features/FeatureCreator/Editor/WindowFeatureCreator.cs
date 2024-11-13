using CustomWindowCreator.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FeatureCreator.Editor
{
    public class WindowFeatureCreator : CustomWindow
    {
        private string _featureName;

        private const string _defaultPath = "Assets/_/Features";
        private const string Data = "Data";
        private const string Editor = "Editor";
        private const string Runtime = "Runtime";

        private bool _hasData = true;
        private bool _hasEditor = true;
        private bool _hasRuntime = true;


        [MenuItem("Assets/Create/Feature", priority = -240)]
        public static void ShowWindow()
        {
            WindowFeatureCreator window = (WindowFeatureCreator)GetWindow(typeof(WindowFeatureCreator));
            window.titleContent = new GUIContent("Feature Creator");
        }

        protected override void Create()
        {
            VisualElement header;
            VisualElement body;

            VisualElement textField;

            _root.Add(
                header = new VisualElementBuilder()
                .Add(CreateLabel("Feature Creator"))
                .Add(textField = CreateTextField("Feature name:").AddListener((evt) => _featureName = evt.newValue))
                .Add(CreateButton("Create new feature").AddListener(CreateFeature))
                .Build());

            _root.Add(
                body = new VisualElementBuilder()
                .Add(CreateToggle(_hasData, "Data folder").AddListener((evt) => _hasData = evt.newValue))
                .Add(CreateToggle(_hasEditor, "Editor folder").AddListener((evt) => _hasEditor = evt.newValue))
                .Add(CreateToggle(_hasRuntime, "Runtime folder").AddListener((evt) => _hasRuntime = evt.newValue))
                .Build());

            textField.Focus();
            textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                CreateFeature();
        }

        //private void OnGUI()
        //{
        //    GUILayout.Label("Feature Creator", EditorStyles.boldLabel);

        //    _featureName = EditorGUILayout.TextField("Feature name:", _featureName);

        //    if (GUILayout.Button("Create new feature")) CreateFeature();

        //    GUILayout.Space(10);
        //    _hasData = EditorGUILayout.Toggle("Data folder", _hasData);
        //    _hasEditor = EditorGUILayout.Toggle("Editor folder", _hasEditor);
        //    _hasRuntime = EditorGUILayout.Toggle("Runtime folder", _hasRuntime);

        //    if (Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
        //        CreateFeature();
        //}


        private void CreateFeature()
        {
            var mainFolderPath = CreateFolder(_featureName, _defaultPath);

            if (_hasData)
                CreateAssemblyDefinition(
                    $"{_featureName}.{Data}",
                    CreateFolder(Data, mainFolderPath),
                    false);

            if (_hasEditor)
                CreateAssemblyDefinition(
                    $"{_featureName}.{Editor}",
                    CreateFolder(Editor, mainFolderPath),
                    true);

            if (_hasRuntime)
                CreateAssemblyDefinition(
                    $"{_featureName}.{Runtime}",
                    CreateFolder(Runtime, mainFolderPath),
                    false);

            Debug.Log($"Assembly Definition Created: <color{_featureName}</color>");
            //EditorUtility.DisplayDialog("Assembly Definition Created", $"{_featureName} created", "OK");
        }

        private string CreateFolder(string name, string path)
        {
            string folderPath = $"{path}/{name}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private void CreateAssemblyDefinition(string name, string folderPath, bool isEditorOnly = false)
        {
            string asmdefPath = $"{folderPath}/{name}.asmdef";

            AssemblyDefinitionAsset assemblyDef = new(name, isEditorOnly);
            string json = JsonUtility.ToJson(assemblyDef, true);

            File.WriteAllText(asmdefPath, json);
            AssetDatabase.Refresh();
        }
    }
}