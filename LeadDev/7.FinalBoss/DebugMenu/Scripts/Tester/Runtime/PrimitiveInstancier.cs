using DebugMenu.Runtime;
using UnityEngine;

namespace Tester.Runtime
{
    public class PrimitiveInstancier : MonoBehaviour
    {
        [SerializeField] private Material _material;

        [DebugMenu("Instantiate/Cube")]
        private void InstanciateCube() => InstantiatePrimitive(PrimitiveType.Cube);

        [DebugMenu("Instantiate/Sphere")]
        private void InstanciateSphere() => InstantiatePrimitive(PrimitiveType.Sphere);

        [DebugMenu("Instantiate/Capsule")]
        private void InstanciateCapsule() => InstantiatePrimitive(PrimitiveType.Capsule);

        [DebugMenu("Instantiate/Cylinder")]
        private void InstanciateCylinder() => InstantiatePrimitive(PrimitiveType.Cylinder);

        [DebugMenu("Instantiate/Plane")]
        private void InstanciatePlane() => InstantiatePrimitive(PrimitiveType.Plane);

        [DebugMenu("Instantiate/Quad")]
        private void InstanciateQuad() => InstantiatePrimitive(PrimitiveType.Quad);

        private void InstantiatePrimitive(PrimitiveType primitiveType)
        {
            var go = GameObject.CreatePrimitive(primitiveType);
            go.transform.position = new(_offsetX, 0, 0);
            go.GetComponent<MeshRenderer>().material = _material;
            _offsetX -= 1.5f;
        }

        private float _offsetX;
    }
}