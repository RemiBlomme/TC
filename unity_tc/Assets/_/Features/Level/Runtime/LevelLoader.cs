using Level.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Level.Runtime
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<LevelDataSO> _levelToLoad;
        [SerializeField] private bool _loadOnStart;


        private void Awake()
        {
            if (!_levelToLoad.IsValid())
            {
                Debug.LogWarning($"[LEVEL]: <color=cyan>{nameof(LevelLoader)}</color> doesn't have a <color=cyan>{nameof(_levelToLoad)}</color>");
                return;
            }

            if (_loadOnStart) LoadLevel(_levelToLoad);
        }

        public void LoadLevel(AssetReferenceT<LevelDataSO> newLevel)
        {
            if (!newLevel.IsValid()) return;
            newLevel.LoadAssetAsync().Completed += OnLevelDataLoaded;
        }

        private void OnLevelDataLoaded(AsyncOperationHandle<LevelDataSO> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Game.Runtime.GameState.LoadLevel(handle.Result);
            }
            else Debug.LogError($"<color=red>[ADDRESSABLE]:</color> Addressable scene <color=cyan>{handle.DebugName}</color> can't load.");
        }
    }
}