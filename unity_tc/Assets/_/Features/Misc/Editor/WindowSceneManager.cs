using Level.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

using static Misc.Editor.CustomUI;

namespace Misc.Editor
{
    public class WindowSceneManager : EditorWindow
    {
        private static WindowSceneManager _window;
        private LevelDataSO _levelData;
        private VisualElement _previousTab;

        private VisualElement _addSceneTab;
        private string _sceneName;

        private VisualElement _removeSceneTab;
        private AssetReference[] _scenesAssetReferences;
        private Toggle[] _toggles;


        public static void ShowWindow(LevelDataSO levelData)
        {
            _window = GetWindow<WindowSceneManager>("Scene Creator");
            _window._levelData = levelData;
            _window.OnCreateGUI();
        }

        private void OnCreateGUI()
        {
            DrawHeader();
            DrawAddSceneTab();
            DrawRemoveSceneTab();

            HideTab(_removeSceneTab);
            _previousTab = _addSceneTab;
        }

        private void ShowTab(VisualElement tab)
        {
            if (tab == null || _previousTab == tab) return;

            if (_previousTab != null) HideTab(_previousTab);
            tab.style.display = DisplayStyle.Flex;
            _previousTab = tab;
        }
        private void HideTab(VisualElement tab)
        {
            if (tab == null) return;
            tab.style.display = DisplayStyle.None;
        }


        #region Header
        private void DrawHeader()
        {
            VisualElement header;
            Button addSceneButton;
            Button removeSceneButton;

            rootVisualElement.Add(
                header = new VisualElementBuilder()
                .Add(addSceneButton = CreateButton("Add").AddListener(() => ShowTab(_addSceneTab)))
                .Add(removeSceneButton = CreateButton("Remove").AddListener(() => ShowTab(_removeSceneTab)))
                .Build());

            rootVisualElement.Add(CreateSpace(minHeight: 20));

            header.style.flexDirection = FlexDirection.Row;
            header.style.justifyContent = Justify.Center;
            header.style.minHeight = 30;
            header.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            header.style.borderBottomColor = new Color(0.1f, 0.1f, 0.1f);
            header.style.borderBottomWidth = 5;

            addSceneButton.style.width = Length.Percent(50);
            addSceneButton.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            removeSceneButton.style.width = Length.Percent(50);
            removeSceneButton.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        }
        #endregion


        #region Add Scene
        private void DrawAddSceneTab()
        {
            TextField textField;

            rootVisualElement.Add(
                _addSceneTab = new VisualElementBuilder()
                .Add(textField = CreateTextField("Scene name:").AddListener((evt) => _sceneName = evt.newValue))
                .Add(CreateButton("Create new scene").AddListener(OnButtonCreateScene))
                .Build());

            textField.Focus();
        }

        private void OnButtonCreateScene() => CreateScene();
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

            Close();
        }
        #endregion


        #region Remove Scene
        private void DrawRemoveSceneTab()
        {
            _scenesAssetReferences = _levelData ? _levelData.GetScenes() : new AssetReference[0];
            _toggles = new Toggle[_scenesAssetReferences.Length];

            _removeSceneTab = new VisualElement();
            ScrollView scrollView = new ScrollView();

            for (int i = 0; i < _scenesAssetReferences.Length; i++)
            {
                AddElement(scrollView, _scenesAssetReferences[i], out Toggle toggle);
                _toggles[i] = toggle;
            }
            _removeSceneTab.Add(scrollView);

            _removeSceneTab.Add(new VisualElementBuilder()
                .Add(CreateButton("Remove selected", OnRemoveSelected))
                .Build());

            rootVisualElement.Add(_removeSceneTab);
        }

        private void OnRemoveSelected()
        {
            ConfirmationPopup.ShowConfirmPopup().Confirmed += OnDeletionConfirmed;
        }

        private void OnDeletionConfirmed(bool isConfirmed)
        {
            if (!isConfirmed) return;

            for (int i = 0; i < _toggles.Length; i++)
            {
                if (!_toggles[i].value) continue;

                AddressableAssetSettingsDefaultObject.Settings.RemoveAssetEntry(_scenesAssetReferences[i].AssetGUID);
                string path = AssetDatabase.GUIDToAssetPath(_scenesAssetReferences[i].AssetGUID);

                AssetDatabase.DeleteAsset(path);
                _levelData.RemoveScene(_scenesAssetReferences[i]);
            }

            Close();
        }

        private void AddElement(VisualElement parent, AssetReference sceneAsset, out Toggle toggle)
        {
            VisualElement newBody = new VisualElementBuilder()
                .Add(toggle = CreateToggle().AddListener(e => OnToggle(e.newValue)))
                .Add(CreateButton(sceneAsset.editorAsset.name))
                .Build();

            newBody.style.flexDirection = FlexDirection.Row;

            parent.Add(newBody);
        }

        private void OnToggle(bool toggled)
        {

        }
        #endregion
    }
}