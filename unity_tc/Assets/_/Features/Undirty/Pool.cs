using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Undirty
{
    public class Pool
    {
        public int Capacity;
        public Action<GameObject, Pool> InstanceCreated;

        private AssetReference _assetReference;
        private Stack<GameObject> _stack;
        private List<Task<GameObject>> _remaningTaskList = new();

        public Pool(AssetReference assetReference, int capacity, Action<Pool> callback = null)
        {
            _assetReference = assetReference;
            _stack = new Stack<GameObject>(capacity);

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

        public void Push(GameObject obj)
        {
            _stack.Push(obj);
        }

        public void Pop(Action<GameObject> callback)
        {
            if (_stack.Count == 0)
            {
                ExtendTo(Capacity + 1, OnExtendCompleted);
                Debug.LogWarning($"<color=yellow>[POOL MANAGER]: The pool size of the {AssetDatabase.GUIDToAssetPath(_assetReference.AssetGUID)} was resized.</color>");

                void OnExtendCompleted(Pool pool) => GetItem(callback);
            }
            else GetItem(callback);
        }

        private void GetItem(Action<GameObject> callback)
        {
            GameObject go = _stack.Pop();
            go.SetActive(true);
            callback?.Invoke(go);
        }

        public bool TryPop(out GameObject obj)
        {
            if (_stack.TryPop(out GameObject item))
            {
                obj = item;
                return true;
            }
            obj = null;
            return false;
        }

        private void Prepare(int amount = 1, Action<Pool> callback = null)
        {
            int remaining = amount;

            for (int i = 0; i < amount; i++)
            {
                var handle = _assetReference.InstantiateAsync();
                handle.Completed += OnAssetPrepared;
                _remaningTaskList.Add(handle.Task);

                void OnAssetPrepared(AsyncOperationHandle<GameObject> handle)
                {
                    handle.Result.SetActive(false);
                    _stack.Push(handle.Result);
                    InstanceCreated?.Invoke(handle.Result, this);

                    if (--remaining <= 0) callback?.Invoke(this);
                }
            }
        }

        //private async void WaitPoolCreated(/*Action onCompleted*/)
        //{
        //    await Task.WhenAll(_remaningTaskList);
        //    _remaningTaskList.Clear();
        //    onCompleted?.Invoke();
        //}

        public async void WaitItemPoolCreated(Action onCompleted)
        {
            await Task.WhenAny(_remaningTaskList);
            onCompleted?.Invoke();

            await Task.WhenAll(_remaningTaskList);
            _remaningTaskList.Clear();
        }
    }
}