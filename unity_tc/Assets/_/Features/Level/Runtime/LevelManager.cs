using UnityEngine;
using UnityEngine.SceneManagement;
using Level.Data;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Level.Runtime
{
    public class LevelManager
    {
        public static LevelDataSO CurrentLevelLoaded;


        /// <summary>Close previous level and open level.</summary>
        public static void ChangeLevelFromAssetReference(AssetReferenceT<LevelDataSO> assetReference)
        {
            assetReference.LoadAssetAsync().Completed +=
                (handle) => ChangeLevel(handle.Result);
        }

        public static void ChangeLevel(LevelDataSO level)
        {
            if (CurrentLevelLoaded) CloseLevel(CurrentLevelLoaded);
            OpenLevel(level);
            CurrentLevelLoaded = level;
        }


        /// <summary>Open all referenced scenes.</summary>
        public static void OpenLevel(LevelDataSO level)
        {
            var levelScenes = level.GetScenes();
            var activeScene = level.GetActiveScene();

            if (Application.isPlaying)
            {
                for (int i = 0; i < levelScenes.Length; i++)
                {
                    levelScenes[i].LoadSceneAsync(LoadSceneMode.Additive);
                }

                if (activeScene.AssetGUID != "")
                    activeScene.LoadSceneAsync(LoadSceneMode.Additive)
                        .Completed += (evt) => SceneManager.SetActiveScene(evt.Result.Scene);
                return;
            }

#if UNITY_EDITOR

            for (int i = 0; i < levelScenes.Length; i++)
            {
                EditorSceneManager.OpenScene(
                    AssetDatabase.GUIDToAssetPath(levelScenes[i].AssetGUID), OpenSceneMode.Additive);
            }

            if (activeScene.AssetGUID != "")
                SceneManager.SetActiveScene(EditorSceneManager.OpenScene(
                    AssetDatabase.GUIDToAssetPath(activeScene.AssetGUID), OpenSceneMode.Additive));
#endif
        }


        /// <summary>Close all referenced scenes.</summary>
        public static void CloseLevel(LevelDataSO level)
        {
            var levelScenes = level.GetScenes();
            var activeScene = level.GetActiveScene();

            if (Application.isPlaying)
            {
                for (int i = levelScenes.Length - 1; i > -1; i--)
                {
                    levelScenes[i].UnLoadScene();
                }

                if (activeScene.AssetGUID != "") activeScene.UnLoadScene();
            }

#if UNITY_EDITOR
            else
            {
                for (int i = levelScenes.Length - 1; i > -1; i--)
                {
                    EditorSceneManager.CloseScene(
                        SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(levelScenes[i].AssetGUID)), true);
                }

                if (activeScene.AssetGUID != "")
                    EditorSceneManager.CloseScene(
                        SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(activeScene.AssetGUID)), true);
            }
#endif
            CurrentLevelLoaded = null;
        }




        //public static void LoadLevel(AssetReferenceT<LevelDataSO> newLevel)
        //{
        //    if (!newLevel.IsValid())
        //    {
        //        Debug.LogError("<color=red>[ADDRESSABLE]: The level you are trying to load is not valid.</color>");
        //        return;
        //    }
        //    newLevel.LoadAssetAsync().Completed += OnLevelDataLoaded;
        //}

        //private static void OnLevelDataLoaded(AsyncOperationHandle<LevelDataSO> handle)
        //{
        //    if (handle.Status == AsyncOperationStatus.Succeeded)
        //    {
        //        if (CurrentLevelLoaded) CloseLevel(CurrentLevelLoaded);
        //        OpenLevel(handle.Result);
        //        CurrentLevelLoaded = handle.Result;
        //    }
        //    else Debug.LogError($"<color=red>[ADDRESSABLE]: Addressable scene <color=cyan>{handle.Result}</color> can not load.</color>");
        //}
    }
}