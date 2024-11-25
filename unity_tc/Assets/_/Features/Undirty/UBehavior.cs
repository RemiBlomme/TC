using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;

namespace UnDirty
{
    public class UBehavior : MonoBehaviour
    {
        [SerializeField] protected bool _isDebug;
        [SerializeField] protected bool _isVerbose;


        public static void Spawn(AssetReference original, Action<Object> callback = null, int poolSize = 0)
        {
            Spawn(original, Vector3.zero, Quaternion.identity, null, callback, poolSize);
        }

        public static void Spawn(AssetReference original, Transform parent, bool instantiateInWorldSpace, Action<Object> callback = null, int poolSize = 0)
        {
            Vector3 position = instantiateInWorldSpace ? Vector3.zero : parent.position;
            Quaternion rotation = instantiateInWorldSpace ? Quaternion.identity : parent.rotation;

            Spawn(original, position, rotation, parent, callback, poolSize);
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Action<Object> callback = null, int poolSize = 0)
        {
            Spawn(original, position, rotation, null, callback, poolSize);
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Transform parent, Action<Object> callback = null, int poolSize = 0)
        {
            original.InstantiateAsync(position, rotation, parent).Completed += OnSpawn;

            void OnSpawn(AsyncOperationHandle<GameObject> handle)
            {
                callback?.Invoke(handle.Result);
            }
        }

        public static void Release()
        {

        }


        protected void VerboseLog(string message, Object context = null)
        {
            if (_isVerbose) Debug.Log(message, context);
        }

        protected void VerboseWarning(string message, Object context = null)
        {
            if (_isVerbose) Debug.Log(message, context);
        }
        protected void VerboseError(string message, Object context = null)
        {
            if (_isVerbose) Debug.Log(message, context);
        }
    }
}