using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Level.Data
{
    [CreateAssetMenu(menuName = "SO/LevelSO")]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] private AssetReference[] SceneAssetReferences;


        /// <summary>Open all referenced scenes at Runtime</summary>
        public void LoadLevel(bool asSingle = false)
        {
            for (int i = 0; i < SceneAssetReferences.Length; i++)
            {
                //if (!IsSceneValide(AssetDatabase.GUIDFromAssetPath(scenePath))) continue;

                SceneAssetReferences[i].LoadSceneAsync(asSingle && i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
            }
        }

        /// <summary>Open all referenced scenes at Runtime</summary>
        public void UnloadLevel()
        {
            for (int i = SceneAssetReferences.Length - 1; i > -1; i--)
            {
                SceneAssetReferences[i].UnLoadScene();
            }
        }


#if UNITY_EDITOR
        /// <summary>Open all referenced scenes in Editor</summary>
        public void OpenEditorLevel(bool asSingle = false)
        {
            for (int i = 0; i < SceneAssetReferences.Length; i++)
            {
                //if (!IsSceneValide(AssetDatabase.GUIDFromAssetPath(scenePath))) continue;

                EditorSceneManager.OpenScene(
                        AssetDatabase.GUIDToAssetPath(SceneAssetReferences[i].AssetGUID),
                        asSingle && i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
            }
        }

        /// <summary>Open all referenced scenes in Editor</summary>
        public void CloseEditorLevel()
        {
            for (int i = SceneAssetReferences.Length - 1; i > -1; i--)
            {
                EditorSceneManager.CloseScene(
                    SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(SceneAssetReferences[i].AssetGUID)), true);
            }
        }
#endif
    }
}