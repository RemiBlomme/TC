using InputManager.Runtime;
using Level.Runtime;
using System.Collections.Generic;

namespace Game.Runtime
{
    public static class TaskManager
    {
        private static List<Task> _taskList = new();

        public static void RegisterTask(Task task)
        {
            task.InputPriorityChanged += OnInputPriorityChanged;
            _taskList.Add(task);

            var scenesData = LevelManager.CurrentLevelLoaded.GetSceneData();

            for (int i = 0; i < scenesData.Length; i++)
            {
                if (task.gameObject.scene.name == scenesData[i].SceneAssetReference.editorAsset.name)
                {
                    task.InputPriority = scenesData[i].InputPriority;
                }
            }
        }

        public static void UnregisterTask(Task task)
        {
            task.InputPriorityChanged -= OnInputPriorityChanged;
            _taskList.Remove(task);
        }

        private static void OnInputPriorityChanged(int priority) => SetInputStates();

        private static void SetInputStates()
        {
            var highPriorityTaskManager = GetTaskWithBiggerInputPriority();

            for (int i = 0; i < _taskList.Count; i++)
            {
                if (_taskList[i] != highPriorityTaskManager) _taskList[i].InputStates = UInputManager.Instance.InputStates;
            }
        }

        private static Task GetTaskWithBiggerInputPriority()
        {
            Task taskManager = _taskList[0];

            for (int i = 0; i < _taskList.Count; i++)
            {
                if (taskManager.InputPriority < _taskList[i].InputPriority)
                {
                    taskManager = _taskList[i];
                }
            }
            return taskManager;
        }
    }
}