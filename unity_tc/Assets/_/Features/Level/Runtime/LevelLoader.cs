using Level.Data;
using UnDirty;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Level.Runtime
{
    public class LevelLoader : UBehaviour
    {
        [SerializeField] private AssetReferenceT<LevelDataSO> _sceneToLoad;
        [SerializeField] private bool _loadOnAwake;

        private void Awake()
        {
            if (_loadOnAwake) LoadLevel(_sceneToLoad);
        }

        public void LoadLevel(AssetReferenceT<LevelDataSO> level) => LevelManager.ChangeLevelFromAssetReference(level);
    }
}