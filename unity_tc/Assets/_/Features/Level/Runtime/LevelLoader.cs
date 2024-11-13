using Level.Data;
using UnityEngine;

namespace Level.Runtime
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private LevelDataSO _levelData;

        private void Awake()
        {
            _levelData.LoadLevel();
        }

        private void OnGUI()
        {
            if(GUILayout.Button("Unload Level")) _levelData.UnloadLevel();
        }
    }
}