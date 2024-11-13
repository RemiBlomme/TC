using Level.Data;
using UnityEditor;
using UnityEditor.Callbacks;


namespace Level.Editor
{
    [CustomEditor(typeof(LevelDataSO))]
    public static class OnOpenLevelSO
    {
        private static LevelDataSO _lastActiveLevelData;


        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceId, int line, int row)
        {
            var current = EditorUtility.InstanceIDToObject(instanceId);
            if (current is not LevelDataSO) return false;

            switch (current)
            {
                case LevelDataSO levelSO:
                    levelSO.OpenEditorLevel();
                    _lastActiveLevelData?.CloseEditorLevel();
                    _lastActiveLevelData = levelSO;
                    return true;

                default:
                    return false;
            }
        }
    }
}