using InputManager.Runtime;
using System;
using UnDirty;

namespace Game.Runtime
{
    public class Task : UBehaviour
    {
        public Action<int> InputPriorityChanged;
        public InputStates InputStates;
        public int InputPriority
        {
            get => _inputPriority;
            set
            {
                _inputPriority = value;
                InputPriorityChanged?.Invoke(_inputPriority);
            }
        }

        private int _inputPriority;


        private void OnEnable()
        {
            TaskManager.RegisterTask(this);
        }

        private void OnDisable()
        {
            TaskManager.UnregisterTask(this);
        }
    }
}