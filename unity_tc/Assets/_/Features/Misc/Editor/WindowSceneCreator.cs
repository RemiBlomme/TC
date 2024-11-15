using Level.Data;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Misc.Editor
{
    public class WindowSceneCreator : EditorWindow
    {
        private static WindowSceneCreator _window;
        private string _sceneName;
        private LevelDataSO _levelData;


        public static void ShowWindow(LevelDataSO levelData)
        {
            _window = GetWindow<WindowSceneCreator>("Scene Creator");
            //_window.position = new Rect(Screen.width * .5f, Screen.height * .5f, 350, 50);

            _window._levelData = levelData;
        }

        private void CreateGUI()
        {
            VisualElement header;
            VisualElement textField;

            rootVisualElement.Add(
                header = new VisualElementBuilder()
                .Add(textField = CustomUI.CreateTextField("Scene name:").AddListener((evt) => _sceneName = evt.newValue))
                .Add(CustomUI.CreateButton("Create new scene").AddListener(OnButtonCreateScene))
                .Build());

            textField.Focus();
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                OnButtonCreateScene();
        }


        private void OnButtonCreateScene()
        {
            CreateScene();
        }

        private void CreateScene()
        {
            var path = $"{AssetDatabase.GetAssetPath(_levelData)}".Replace($"{_levelData.name}.asset", "Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SaveScene(scene, $"{path}\\{_sceneName}.unity");

            AssetDatabase.Refresh();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(scene.path), settings.DefaultGroup);
            AssetReference assetReference = new(entry.guid);

            EditorSceneManager.CloseScene(scene, true);

            _levelData.AddScene(assetReference);

            Debug.Log($"New scene Created: <color=cyan>{_sceneName}</color>");
            GUIContent notification = new("Scene Created");
            SceneView.lastActiveSceneView.ShowNotification(notification);

            AssetDatabase.Refresh();

            _window.Close();
        }
    }


    public class WindowSceneRemove : EditorWindow
    {
        private static WindowSceneRemove _window;
        private string _sceneName;
        private LevelDataSO _levelData;

        private List<Scene> _scenesToRemoves = new();


        public static void ShowWindow(LevelDataSO levelData)
        {
            _window = GetWindow<WindowSceneRemove>("Scene Remover");
            _window._levelData = levelData;
            _window.DrawUI();
        }

        private void DrawUI()
        {
            var scenes = _levelData ? _levelData.GetScenes() : new AssetReference[0];
            for (int i = 0; i < scenes.Length; i++)
            {
                //AddElement(rootVisualElement, scenes[i].editorAsset.name);
            }

            rootVisualElement.Add(
                CustomUI.CreateButton("Remove selected").AddListener(OnRemoveSelected));
        }

        private void OnRemoveSelected()
        {
            throw new NotImplementedException();
        }

        private void AddElement(VisualElement root, Scene scene)
        {
            VisualElement newBody = new VisualElementBuilder()
                .Add(CustomUI.CreateToggle().AddListener(e => { if (e.newValue) OnToggle(scene); }))
                .Add(CustomUI.CreateButton(scene.name))
                .Build();
            newBody.style.flexDirection = FlexDirection.Row;

            root.Add(newBody);
        }

        private void OnToggle(Scene scene)
        {
            //_scenesToRemoves.Add()
            //evt
            
        }

        //private void OnButtonCreateScene()
        //{
        //}
    }
}