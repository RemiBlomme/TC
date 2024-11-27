using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Undirty
{
    public class Pool
    {
        public int Capacity;
        public Action<GameObject, Pool> OnGetItem;

        private AssetReference _assetReference;
        private Stack<GameObject> _stack;
        private List<Task<GameObject>> _remaningTaskList = new();

        public Pool(AssetReference assetReference, int capacity, Action<Pool> callback = null)
        {
            _assetReference = assetReference;
            _stack = new(capacity);

            ExtendTo(capacity, callback);
        }


        public void ExtendTo(int capacity, Action<Pool> callback = null)
        {
            if (Capacity >= capacity)
            {
                callback?.Invoke(this);
                return;
            }

            int add = capacity - Capacity;
            Capacity = capacity;

            Prepare(add, callback);
        }

        public void Push(GameObject go)
        {
            go.SetActive(false);
            _stack.Push(go);
        }

        public void PopAsync(Action<GameObject> callback)
        {
            if (_stack.Count == 0)
            {
                ExtendTo(Capacity + 1, OnExtendCompleted);
#if UNITY_EDITOR
                Debug.LogWarning($"<color=yellow>[POOL MANAGER]: The pool size of the {AssetDatabase.GUIDToAssetPath(_assetReference.AssetGUID)} was resized.</color>");
#endif
                void OnExtendCompleted(Pool pool) => GetItem(callback);
            }
            else GetItem(callback);
        }

        private void GetItem(Action<GameObject> callback)
        {
            GameObject go = _stack.Pop();
            go.SetActive(true);
            OnGetItem?.Invoke(go, this);
            callback?.Invoke(go);
        }

        public bool TryPop(out GameObject go)
            => _stack.TryPop(out go);

        private void Prepare(int amount = 1, Action<Pool> callback = null)
        {
            int remaining = amount;

            for (int i = 0; i < amount; i++)
            {
                var handle = _assetReference.InstantiateAsync();
                handle.Completed += OnAssetPrepared;
                _remaningTaskList.Add(handle.Task);
            }

            void OnAssetPrepared(AsyncOperationHandle<GameObject> handle)
            {
                _stack.Push(handle.Result);
                

                if (--remaining <= 0) callback?.Invoke(this);
            }
        }

        public async void WaitItemPoolCreated(Action onCompleted)
        {
            if (_remaningTaskList.Count > 0) await Task.WhenAny(_remaningTaskList);
            onCompleted?.Invoke();

            if (_remaningTaskList.Count > 0)
            {
                await Task.WhenAll(_remaningTaskList);
                _remaningTaskList.Clear();
            }
        }
    }
}