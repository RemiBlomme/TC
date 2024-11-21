using Level.Data;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Level.Runtime
{
    public class LevelLoader : MonoBehaviour
    {
        public event Action<AssetReference> LevelLoaded;

        [SerializeField] private AssetReference _levelToLoad;
        private AssetReference _currentLevel;


        private void Start()
        {
            LoadLevel(_levelToLoad);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Unload Level"))
                _currentLevel.LoadAssetAsync<LevelDataSO>().Completed += OnCompletedCLoseLevel;
        }

        public void LoadLevel(AssetReference newLevel)
        {
            _currentLevel.LoadAssetAsync<LevelDataSO>().Completed += OnCompletedCLoseLevel;
            newLevel.LoadAssetAsync<LevelDataSO>().Completed += OnCompletedOpenLevel;
            _currentLevel = newLevel;

            LevelLoaded?.Invoke(_currentLevel);
        }

        private void OnCompletedOpenLevel(AsyncOperationHandle<LevelDataSO> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                handle.Result.OpenLevel();
            }
            else Debug.LogWarning("[ADDRESSABLE]: Addressable can not load");
        }

        private void OnCompletedCLoseLevel(AsyncOperationHandle<LevelDataSO> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                handle.Result.CloseLevel();
            }
            else Debug.LogWarning("[ADDRESSABLE]: Addressable can not load");
        }
    }
}