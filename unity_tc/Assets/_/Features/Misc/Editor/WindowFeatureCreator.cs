using Misc.Data;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misc.Editor
{
    public class WindowFeatureCreator : EditorWindow
    {
        private static WindowFeatureCreator _window;
        private string _featureName;

        private const string Data = "Data";
        private const string Editor = "Editor";
        private const string Runtime = "Runtime";

        private bool _hasData = true;
        private bool _hasEditor = true;
        private bool _hasRuntime = true;


        [MenuItem("Assets/Create/Feature _#b", priority = -240)]
        public static void ShowWindow()
        {
            _window = GetWindow<WindowFeatureCreator>("Feature Creator");
            _window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 200);
        }

        private void CreateGUI()
        {
            VisualElement header;
            VisualElement body;

            VisualElement textField;

            rootVisualElement.Add(
                header = new VisualElementBuilder()
                .Add(textField = CustomUI.CreateTextField("Feature name:").AddListener((evt) => _featureName = evt.newValue))
                .Add(CustomUI.CreateButton("Create new feature").AddListener(CreateFeature))
                .Build());

            rootVisualElement.Add(
                body = new VisualElementBuilder()
                .Add(CustomUI.CreateToggle("Data folder", _hasData).AddListener((evt) => _hasData = evt.newValue))
                .Add(CustomUI.CreateToggle("Editor folder", _hasEditor).AddListener((evt) => _hasEditor = evt.newValue))
                .Add(CustomUI.CreateToggle("Runtime folder", _hasRuntime).AddListener((evt) => _hasRuntime = evt.newValue))
                .Build());

            textField.Focus();
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
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

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                CreateFeature();
        }

        private void CreateFeature()
        {
            var mainFolderPath = CreateFolder(_featureName, Paths.FEATURE_FOLDER_PATH);

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

            Debug.Log($"Assembly Definition Created: <color=cyan>{_featureName}</color>");
            GUIContent notification = new ("Assembly Created");
            SceneView.lastActiveSceneView.ShowNotification(notification);

            //EditorGUIUtility.PingObject();

            _window.Close();
        }

        private string CreateFolder(string name, string path)
        {
            string folderPath = $"{path}/{name}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private async void CreateAssemblyDefinition(string name, string folderPath, bool isEditorOnly = false)
        {
            string asmdefPath = $"{folderPath}/{name}.asmdef";

            AssemblyDefinitionAsset assemblyDef = new(name, isEditorOnly);
            string json = JsonUtility.ToJson(assemblyDef, true);

            File.WriteAllText(asmdefPath, json);

            await Task.Delay(100);
            AssetDatabase.Refresh();
        }
    }
}