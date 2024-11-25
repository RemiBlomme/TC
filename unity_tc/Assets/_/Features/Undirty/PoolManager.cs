using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Object = UnityEngine.Object;

namespace Undirty
{
    public static class PoolManager
    {
        private static Dictionary<AssetReference, Stack<Object>> _pools = new();


        public static Object Get(AssetReference assetReference, int poolSize)
        {
            if (_pools.TryGetValue(assetReference, out Stack<Object> pool))
            {
                if (pool.Count < poolSize)
                {
                    int number = poolSize - pool.Count;
                    Prepare(assetReference, number);
                    Debug.LogWarning($"[POOL MANAGER]: The pool size of the {assetReference} was resized by {number} elements.");
                }
            }
            else
            {
                _pools.Add(assetReference, new Stack<Object>(poolSize));
            }

            if (pool.TryPop(out Object result)) return result;

            Prepare(assetReference, 1);
            _pools[assetReference].Push(result);
            
        }

        public static void Release(AssetReference assetReference, Object obj)
        {
            _pools[assetReference].Push(obj);
        }


        public static void Prepare(AssetReference original, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                original.InstantiateAsync().Completed += OnAssetPrepared;
            }

            void OnAssetPrepared(AsyncOperationHandle<GameObject> handle)
            {
                handle.Result.SetActive(false);

                if (!_pools.ContainsKey(original)) _pools.Add(original, new Stack<Object>(amount));
                _pools[original].Push(handle.Result);
            }
        }
    }
}