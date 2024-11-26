using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Undirty
{
    public static class PoolManager
    {
        private static Dictionary<AssetReference, Pool> _pools = new();
        private static Dictionary<GameObject, Pool> _bindings = new();


        public static void GetInstanceOf(AssetReference assetReference, Action<GameObject> callback, int poolCapacity = 0)
        {
            GetPoolAsync(assetReference, poolCapacity, OnGetPool);

            void OnGetPool(Pool pool)
            {
                pool.WaitItemPoolCreated(OnItemPoolCreated);

                void OnItemPoolCreated() => pool.Pop(callback);
            }
        }

        public static void Release(GameObject go)
        {
            _bindings[go].Push(go);
            _bindings.Remove(go);
        }

        public static bool HasPool(AssetReference assetReference)
        {
            return _pools.ContainsKey(assetReference);
        }

        public static void GetPoolAsync(AssetReference assetReference, int poolCapacity, Action<Pool> onGetPool = null)
        {
            if (HasPool(assetReference))
            {
                _pools[assetReference].ExtendTo(poolCapacity, onGetPool);
            }
            else CreatePool(assetReference, poolCapacity, onGetPool);
        }

        public static Pool CreatePool(AssetReference assetReference, int poolCapacity, Action<Pool> callback = null)
        {
            var pool = new Pool(assetReference, poolCapacity, callback);
            pool.InstanceCreated += OnInstanceCreated;
            _pools.Add(assetReference, pool);

            return pool;
        }

        private static void OnInstanceCreated(GameObject go, Pool pool)
            => _bindings.Add(go, pool);
    }
}