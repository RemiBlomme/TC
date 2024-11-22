using UnityEditor;
using UnityEngine.SceneManagement;

namespace ToolBar.Editor
{
    [InitializeOnLoad]
    public class PlayModeOverride
    {
        static PlayModeOverride()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (Levels.GetOverridePlaymode())
                {
                    if (EditorBuildSettings.scenes.Length > 0)
                    {
                        string firstScenePath = EditorBuildSettings.scenes[0].path;
                        string sceneName = firstScenePath.Substring(firstScenePath.LastIndexOf("/") + 1).Replace(".unity", "");
                        SceneManager.LoadSceneAsync(sceneName);
                    }

                    // CORRECT THIS 
                    Levels.GetLevelSelected().OpenLevel();
                }

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    
                }
            }
        }
    }
}