using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Misc.Runtime;
using Level.Data;
using UnityEditor.AddressableAssets;

namespace Misc.Editor
{
    public class WindowFeatureCreator : CustomWindow
    {
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
            //textField.RegisterCallback<KeyDownEvent>(OnKeyDown);
            _root.RegisterCallback<KeyDownEvent>(OnKeyDown);
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
            //EditorUtility.DisplayDialog("Assembly Definition Created", $"{_featureName} created", "OK");

            GUIContent notification = new ("Assembly Created");
            SceneView.lastActiveSceneView.ShowNotification(notification);
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





    public class WindowLevelCreator : CustomWindow
    {
        private string _levelName;


        [MenuItem("Assets/Create/Level", priority = -240)]
        public static void ShowWindow()
        {
            WindowLevelCreator window = (WindowLevelCreator)GetWindow(typeof(WindowLevelCreator));
            window.titleContent = new GUIContent("Level Creator");
        }

        protected override void Create()
        {
            VisualElement header;

            VisualElement textField;

            _root.Add(
                header = new VisualElementBuilder()
                .Add(CreateLabel("Level Creator"))
                .Add(textField = CreateTextField("Level name:").AddListener((evt) => _levelName = evt.newValue))
                .Add(CreateButton("Create new level").AddListener(CreateLevel))
                .Build());

            textField.Focus();
            _root.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                CreateLevel();
        }


        private void CreateLevel()
        {
            var mainFolderPath = CreateFolder(_levelName, Paths.LEVEL_FOLDER_PATH);
            CreateFolder("scenes", mainFolderPath);
            CreateLevelData(_levelName, mainFolderPath);

            Debug.Log($"New Level Created: <color=cyan>{_levelName}</color>");

            GUIContent notification = new("Level Created");
            SceneView.lastActiveSceneView.ShowNotification(notification);
        }

        private string CreateFolder(string name, string path)
        {
            string folderPath = $"{path}/{name}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return folderPath;
        }

        private void CreateLevelData(string name, string folderPath)
        {
            string newPath = $"{folderPath}/{name}.asset";

            AssetDatabase.CreateAsset(
                ScriptableObject.CreateInstance(nameof(LevelDataSO)),
                newPath);

            AssetDatabase.Refresh();
        }
    }
}