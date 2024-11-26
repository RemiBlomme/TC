using System;
using Undirty;
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


        public static void Spawn(AssetReference original, int poolCapacity = 0, Action<Object> callback = null)
        {
            Spawn(original, Vector3.zero, Quaternion.identity, null, poolCapacity, callback);
        }

        public static void Spawn(AssetReference original, Transform parent, bool instantiateInWorldSpace, int poolCapacity = 0, Action<Object> callback = null)
        {
            Vector3 position = instantiateInWorldSpace ? Vector3.zero : parent.position;
            Quaternion rotation = instantiateInWorldSpace ? Quaternion.identity : parent.rotation;

            Spawn(original, position, rotation, parent, poolCapacity, callback);
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, int poolCapacity = 0, Action<Object> callback = null)
        {
            Spawn(original, position, rotation, null, poolCapacity, callback);
        }

        public static void Spawn(AssetReference original, Vector3 position, Quaternion rotation, Transform parent, int poolCapacity = 0, Action<Object> callback = null)
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

                    transform.position = position;
                    transform.rotation = rotation;
                    transform.parent = parent;

                    callback?.Invoke(go);
                }
            }
        }

        public static void Release(GameObject go)
        {
            PoolManager.Release(go);
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