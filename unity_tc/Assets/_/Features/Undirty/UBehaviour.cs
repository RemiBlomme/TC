using System;
using System.Diagnostics;
using Undirty;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;

namespace UnDirty
{
    public class UBehaviour : MonoBehaviour
    {
        #region Publics

        [SerializeField] public bool IsDebug;
        [SerializeField] public bool IsVerbose;

        #endregion


        #region Cached Members

        [NonSerialized]
        private Transform _transform;
        public new Transform transform => _transform ??= GetComponent<Transform>();

        [NonSerialized]
        private Animation _animation;
        public new Animation animation => _animation ??= GetComponent<Animation>();

        [NonSerialized]
        private Camera _camera;
        public new Camera camera => _camera ??= GetComponent<Camera>();

        [NonSerialized]
        private Collider _collider;
        public new Collider collider => _collider ??= GetComponent<Collider>();

        [NonSerialized]
        private Collider2D _collider2D;
        public new Collider2D collider2D => _collider2D ??= GetComponent<Collider2D>();

        [NonSerialized]
        private ConstantForce _constantForce;
        public new ConstantForce constantForce => _constantForce ??= GetComponent<ConstantForce>();

        [NonSerialized]
        private HingeJoint _hingeJoint;
        public new HingeJoint hingeJoint => _hingeJoint ??= GetComponent<HingeJoint>();

        [NonSerialized]
        private Light _light;
        public new Light light => _light ??= GetComponent<Light>();

        [NonSerialized]
        private ParticleSystem _particleSystem;
        public new ParticleSystem particleSystem => _particleSystem ??= GetComponent<ParticleSystem>();

        [NonSerialized]
        private Renderer _renderer;
        public new Renderer renderer => _renderer ??= GetComponent<Renderer>();

        [NonSerialized]
        private Rigidbody _rigidbody;
        public new Rigidbody rigidbody => _rigidbody ??= GetComponent<Rigidbody>();

        [NonSerialized]
        private Rigidbody2D _rigidbody2D;
        public new Rigidbody2D rigidbody2D => _rigidbody2D ??= GetComponent<Rigidbody2D>();

        #endregion


        public static void Spawn(AssetReference original, Action<Object> callback = null, int poolCapacity = 0)
            => Spawn(original, Vector3.zero, Quaternion.identity, null, callback, poolCapacity);

        public static void Spawn(AssetReference original, Transform parent, Action<Object> callback = null, int poolCapacity = 0)
            => Spawn(original, Vector3.zero, Quaternion.identity, parent, callback, poolCapacity);

        public static void Spawn(AssetReference original, Transform parent, bool instantiateInWorldSpace, Action<Object> callback = null, int poolCapacity = 0)
        {
            Vector3 position = instantiateInWorldSpace ? Vector3.zero : parent.position;
            Quaternion rotation = instantiateInWorldSpace ? Quaternion.identity : parent.rotation;

            Spawn(original, position, rotation, parent, callback, poolCapacity);
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Action<Object> callback = null, int poolCapacity = 0)
            => Spawn(original, position, rotation, null, callback, poolCapacity);

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Transform parent, Action<Object> callback = null, int poolCapacity = 0)
        {
            if (poolCapacity <= 0 && !PoolManager.HasPool(original))
            {
                original.InstantiateAsync(position, rotation, parent).Completed += OnSpawn;

                void OnSpawn(AsyncOperationHandle<GameObject> handle)
                {
                    callback?.Invoke(handle.Result);
                }
            }
            else
            {
                PoolManager.GetInstanceOf(original, OnSpawn, poolCapacity);

                void OnSpawn(GameObject go)
                {
                    var transform = go.transform;

                    transform.SetPositionAndRotation(position, rotation);
                    transform.parent = parent;

                    callback?.Invoke(go);
                }
            }
        }

        public static void Release(GameObject go) => PoolManager.Release(go);


        #region Verbose

        [Conditional("DEBUG")]
        protected void VerboseLog(object message, Object context = null)
        {
            if (IsVerbose) UnityEngine.Debug.Log(message, context);
        }

        [Conditional("DEBUG")]
        protected void VerboseWarning(object message, Object context = null)
        {
            if (IsVerbose) UnityEngine.Debug.LogWarning(message, context);
        }

        [Conditional("DEBUG")]
        protected void VerboseError(object message, Object context = null)
        {
            if (IsVerbose) UnityEngine.Debug.LogError(message, context);
        }

        #endregion
    }
}