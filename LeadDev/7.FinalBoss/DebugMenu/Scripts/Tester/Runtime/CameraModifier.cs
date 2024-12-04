using DebugMenu.Runtime;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tester.Runtime
{
    public class CameraModifier : MonoBehaviour
    {
        [SerializeField] private Volume _volume;

        private void Update()
        {
            if (_lastValue != _enabled)
            {
                _lastValue = _enabled;
                EnableDisablePostProcess();
            }
        }

        private void EnableDisablePostProcess()
        {
            _volume.enabled = _enabled;
        }

        [DebugMenu("Camera/Post process")] private bool _enabled;
        private bool _lastValue;
    }
}