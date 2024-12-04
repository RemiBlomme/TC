using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Misc.Data;
using Misc.Editor;
using Level.Data;
using Symlink.Editor;
using AddressableDefinition.Editor;

namespace Level.Editor
{
    public class WindowLevelCreator : EditorWindow
    {
        private string _levelName;


        [MenuItem("Assets/Create/Level", priority = -240)]
        public static void ShowWindow()
        {
            var window = GetWindow<WindowLevelCreator>("Level Creator");
            window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 300, 200);
        }

        private void CreateGUI()
        {
            VisualElement header;
            VisualElement textField;

            rootVisualElement.Add(
                header = new VisualElementBuilder()
                .Add(textField = CustomUI.CreateTextField("Level name:").AddListener(OnTextFieldWrite))
                .Add(CustomUI.CreateButton("Create new level").AddListener(CreateLevel))
                .Build());

            textField.Focus();
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnTextFieldWrite(ChangeEvent<string> evt) => _levelName = evt.newValue;

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

            SymlinkManager.MoveToSymlinksFile(mainFolderPath, true);

            Debug.Log($"New Level Created: <color=cyan>{_levelName}</color>");
            GUIContent notification = new("Level Created");
            SceneView.lastActiveSceneView.ShowNotification(notification);

            Close();
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
            AssetDatabase.SaveAssetIfDirty(new GUID(AssetDatabase.AssetPathToGUID(newPath)));

            return so;
        }
    }
}