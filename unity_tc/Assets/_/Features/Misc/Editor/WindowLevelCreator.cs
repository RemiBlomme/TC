using Level.Data;
using Misc.Runtime;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

namespace Misc.Editor
{
    public class WindowLevelCreator : EditorWindow
    {
        private static WindowLevelCreator _window;
        private string _levelName;


        [MenuItem("Assets/Create/Level", priority = -240)]
        public static void ShowWindow()
        {
            _window = GetWindow<WindowLevelCreator>("Level Creator");
            _window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 200);
        }

        private void CreateGUI()
        {
            VisualElement header;
            VisualElement textField;

            rootVisualElement.Add(
                header = new VisualElementBuilder()
                .Add(textField = CustomUI.CreateTextField("Level name:").AddListener((evt) => _levelName = evt.newValue))
                .Add(CustomUI.CreateButton("Create new level").AddListener(CreateLevel))
                .Build());

            textField.Focus();
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
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

            var addressableDefinitionSO = CreateSO($"{_levelName} AddressableDefinition", mainFolderPath, nameof(AddressableDefinitionSO));
            AssetDatabase.SetLabels(addressableDefinitionSO, new[] { "AddressableIgnore" });

            CreateSO(_levelName, mainFolderPath, nameof(LevelDataSO));

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

        private ScriptableObject CreateSO(string name, string folderPath, string className)
        {
            string newPath = $"{folderPath}/{name}.asset";

            var so = CreateInstance(className);
            AssetDatabase.CreateAsset(so, newPath);
            
            //AssetDatabase.Refresh();
            AssetDatabase.SaveAssetIfDirty(new GUID(AssetDatabase.AssetPathToGUID(newPath)));

            return so;
        }
    }
}