using Level.Data;
using Misc.Runtime;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Misc.Editor
{
    public class WindowLevelCreator : CustomWindow
    {
        private static WindowLevelCreator _window;
        private string _levelName;


        [MenuItem("Assets/Create/Level", priority = -240)]
        public static void ShowWindow()
        {
            _window = GetWindow<WindowLevelCreator>("Level Creator");
            _window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 200);
        }

        protected override void Create()
        {
            VisualElement header;

            VisualElement textField;

            _root.Add(
                header = new VisualElementBuilder()
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
            CreateFolder("Scenes", mainFolderPath);
            CreateLevelData(_levelName, mainFolderPath);

            Debug.Log($"New Level Created: <color=cyan>{_levelName}</color>");

            GUIContent notification = new("Level Created");
            SceneView.lastActiveSceneView.ShowNotification(notification);

            _window.Close();
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