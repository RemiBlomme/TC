using Level.Data;
using UnityEngine;

namespace Game.Runtime
{
    public class State : MonoBehaviour
    {
        private static LevelDataSO _currentLevelLoaded;

        public static void LoadLevel(LevelDataSO levelData)
        {
            _currentLevelLoaded?.CloseLevel();
            levelData.OpenLevel();
            _currentLevelLoaded = levelData;
        }
    }
}