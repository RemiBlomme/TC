using Level.Data;
using Level.Runtime;
using UnityEngine;
using UnityEngine.GUID;

namespace GameState.Runtime
{
    public class GameState : MonoBehaviour
    {
        [SerializeField] private GuidReference _levelLoader;

        private LevelDataSO _loadedLevelData;


        private void Awake()
        {
            _levelLoader.gameObject.GetComponent<LevelLoader>().LevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded(LevelDataSO levelData) => _loadedLevelData = levelData;
    }
}