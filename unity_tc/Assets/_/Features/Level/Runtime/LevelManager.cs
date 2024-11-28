using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Level.Data;

namespace Level.Runtime
{
    public class LevelManager
    {
        public static LevelDataSO CurrentLevelLoaded;


        public static void LoadLevel(AssetReferenceT<LevelDataSO> newLevel)
        {
            if (!newLevel.IsValid())
            {
                Debug.LogError("<color=red>[ADDRESSABLE]: The level you are trying to load is not valid.</color>");
                return;
            }
            newLevel.LoadAssetAsync().Completed += OnLevelDataLoaded;
        }

        private static void OnLevelDataLoaded(AsyncOperationHandle<LevelDataSO> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CurrentLevelLoaded?.CloseLevel();
                handle.Result.OpenLevel();
                CurrentLevelLoaded = handle.Result;
            }
            else Debug.LogError($"<color=red>[ADDRESSABLE]: Addressable scene <color=cyan>{handle.Result}</color> can not load.</color>");
        }
    }
}