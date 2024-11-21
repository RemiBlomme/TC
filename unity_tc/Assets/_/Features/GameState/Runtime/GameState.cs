using Level.Runtime;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.GUID;

namespace GameState.Runtime
{
    public class GameState : MonoBehaviour
    {
        [SerializeField] private GuidReference _levelLoader;

        private AssetReference _loadedLevelData;


        private void Awake()
        {
            _levelLoader.gameObject.GetComponent<LevelLoader>().LevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded(AssetReference levelLoaded) => _loadedLevelData = levelLoaded;
    }
}