using System.Collections.Generic;
using UnDirty;

namespace InputManager.Runtime
{
    public class UInputManager : UBehaviour
    {
        private List<TaskManager> _taskManagerList = new();
        private InputStateSetter _inputManager = new();


        private void OnEnable()
        {
            _inputManager.Enable();
        }

        private void OnDisable()
        {
            _inputManager.Disable();
        }


        private void SetInputState()
        {
            var highPriorityTaskManager = GetTaskWithBiggerInputPriority();

            for (int i = 0; i < _taskManagerList.Count; i++)
            {
                _taskManagerList[i].InputStates =
                    highPriorityTaskManager == _taskManagerList[i] ? _inputManager.InputStates : _inputManager.DefaultInputSate;
            }
        }

        private TaskManager GetTaskWithBiggerInputPriority()
        {
            TaskManager taskManager = _taskManagerList[0];

            for (int i = 0; i < _taskManagerList.Count; i++)
            {
                if (taskManager.InputPriority < _taskManagerList[i].InputPriority)
                {
                    taskManager = _taskManagerList[i];
                }
            }
            return taskManager;
        }
    }

    public class TaskManager : UBehaviour
    {
        public int InputPriority { get; }
        public InputStates InputStates;
    }
}