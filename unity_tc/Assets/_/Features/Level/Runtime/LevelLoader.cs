using Level.Data;
using System;
using UnityEngine;

namespace Level.Runtime
{
    public class LevelLoader : MonoBehaviour
    {
        public event Action<LevelDataSO> LevelLoaded;

        [SerializeField] private LevelDataSO _levelToLoad;
        private LevelDataSO _currentLevelData;


        private void Start()
        {
            LoadLevel(_levelToLoad);
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Unload Level")) _currentLevelData?.CloseLevel();
        }

        public void LoadLevel(LevelDataSO newLevelData)
        {
            _currentLevelData?.CloseLevel();
            newLevelData?.OpenLevel();
            _currentLevelData = newLevelData;

            LevelLoaded?.Invoke(_currentLevelData);
        }
    }
}