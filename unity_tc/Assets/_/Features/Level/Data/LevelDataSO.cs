#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;

using LoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;


namespace Level.Data
{
    [CreateAssetMenu(menuName = "SO/LevelSO")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private AssetReference[] SceneAssetReferences;


        /// <summary>Open all referenced scenes</summary>
        public void OpenLevel(bool asSingle = false)
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < SceneAssetReferences.Length; i++)
                {
                    SceneAssetReferences[i].LoadSceneAsync(asSingle && i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
                }
                return;
            }

#if UNITY_EDITOR
            for (int i = 0; i < SceneAssetReferences.Length; i++)
            {
                EditorSceneManager.OpenScene(
                        AssetDatabase.GUIDToAssetPath(SceneAssetReferences[i].AssetGUID),
                        asSingle && i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }
#endif
        }


        /// <summary>Open all referenced scenes</summary>
        public void CloseLevel()
        {
            if (Application.isPlaying)
            {
                for (int i = SceneAssetReferences.Length - 1; i > -1; i--)
                {
                    SceneAssetReferences[i].UnLoadScene();
                }
                return;
            }

#if UNITY_EDITOR
            for (int i = SceneAssetReferences.Length - 1; i > -1; i--)
            {
                EditorSceneManager.CloseScene(
                    SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(SceneAssetReferences[i].AssetGUID)), true);
            }
#endif
        }
    }
}