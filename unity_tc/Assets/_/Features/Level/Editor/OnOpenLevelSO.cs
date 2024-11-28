using Level.Data;
using Level.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Level.Editor
{
    [CustomEditor(typeof(LevelDataSO))]
    public class OnOpenLevelSO : UnityEditor.Editor
    {
        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int line, int row)
        {
            var current = EditorUtility.InstanceIDToObject(instanceId);
            if (current is not LevelDataSO) return false;

            switch (current)
            {
                case LevelDataSO levelSO:
                    if (LevelManager.CurrentLevelLoaded == levelSO)
                    {
                        LevelManager.CloseLevel(levelSO);
                        return true;
                    }
                    LevelManager.ChangeLevel(levelSO);
                    return true;

                default:
                    return false;
            }
        }
    }
}