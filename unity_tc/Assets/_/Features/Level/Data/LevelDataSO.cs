#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using System.Linq;

using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;


namespace Level.Data
{
    [CreateAssetMenu(menuName = "SO/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private AssetReference _lightingSceneAssetReference;
        [SerializeField] private AssetReference[] _sceneAssetReferences;


        public void AddScene(AssetReference assetReference)
        {
            _sceneAssetReferences = _sceneAssetReferences.Append(assetReference).ToArray();
        }
        public void RemoveScene(AssetReference assetReference)
        {
            AssetReference[] newSceneAssetReference = new AssetReference[_sceneAssetReferences.Length - 1];
            int newIndex = 0;

            foreach (var item in _sceneAssetReferences)
            {
                if (item == assetReference) continue;
                newSceneAssetReference[newIndex++] = item;
            }

            _sceneAssetReferences = newSceneAssetReference;
        }
        public AssetReference[] GetScenes() => _sceneAssetReferences;


        /// <summary>Open all referenced scenes</summary>
        public void OpenLevel(bool asSingle = false)
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < _sceneAssetReferences.Length; i++)
                {
                    _sceneAssetReferences[i].LoadSceneAsync(asSingle && i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                }

                _lightingSceneAssetReference.LoadSceneAsync(LoadSceneMode.Additive)
                    .Completed += (evt) => SceneManager.SetActiveScene(evt.Result.Scene);

                return;
            }

#if UNITY_EDITOR
            for (int i = 0; i < _sceneAssetReferences.Length; i++)
            {
                EditorSceneManager.OpenScene(
                        AssetDatabase.GUIDToAssetPath(_sceneAssetReferences[i].AssetGUID),
                        asSingle && i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }

            SceneManager.SetActiveScene(EditorSceneManager.OpenScene(
                AssetDatabase.GUIDToAssetPath(_lightingSceneAssetReference.AssetGUID), OpenSceneMode.Additive));
#endif
        }


        /// <summary>Open all referenced scenes</summary>
        public void CloseLevel()
        {
            if (Application.isPlaying)
            {
                for (int i = _sceneAssetReferences.Length - 1; i > -1; i--)
                {
                    _sceneAssetReferences[i].UnLoadScene();
                }

                _lightingSceneAssetReference.UnLoadScene();

                return;
            }

#if UNITY_EDITOR
            for (int i = _sceneAssetReferences.Length - 1; i > -1; i--)
            {
                EditorSceneManager.CloseScene(
                    SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(_sceneAssetReferences[i].AssetGUID)), true);
            }

            EditorSceneManager.CloseScene(
                SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(_lightingSceneAssetReference.AssetGUID)), true);
#endif
        }
    }
}