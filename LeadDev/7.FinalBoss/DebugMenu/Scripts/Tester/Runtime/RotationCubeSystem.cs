using DebugMenu.Runtime;
using UnityEngine;

namespace Tester.Runtime
{
    public class RotationCubeSystem : MonoBehaviour
    {
        [SerializeField] private Transform[] _transforms;

        private void Update()
        {
            if (!_enableRotation) return;
            var rotationValue = _rotationInverted ? -1 : 1;
            for (int i = 0; i < _transforms.Length; i++)
            {
                var tr = _transforms[i];
                tr.transform.Rotate(Vector3.up, rotationValue * Time.timeScale);
            }
        }

        [DebugMenu("Inverse Rotation")]
        private void InverseRotation() => _rotationInverted = !_rotationInverted;

        [DebugMenu("Rotate Cubes")] private bool _enableRotation = true;
        private bool _rotationInverted;
    }
}