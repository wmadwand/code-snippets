using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.GameObject;

namespace Project.Utils
{
    public interface IMonoObjectPool<T> where T : MonoBehaviour
    {
        void Dispose();
        T Get();
        void Return(T item);
    }

    //TODO: add a feature as poolMaxSize
    public class MonoObjectPool<T> : IDisposable, IMonoObjectPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly Stack<T> _inactiveObjects;
        private readonly bool _activateOnGet;
        private readonly Transform _parent;
        private readonly Action<T> _actionOnGet;
        private readonly Action<T> _actionOnReturn;

        //--------------------------------------------------------------    

        public MonoObjectPool(T prefab, int initialSize = 50, bool activateOnGet = true, Transform parent = null, Action<T> actionOnGet = null, Action<T> actionOnReturn = null)
        {
            _prefab = prefab;
            _inactiveObjects = new Stack<T>(initialSize);
            _activateOnGet = activateOnGet;
            _parent = parent;
            _actionOnGet = actionOnGet;
            _actionOnReturn = actionOnReturn;
        }

        public T Get()
        {
            T result = null;
            while (_inactiveObjects.Count > 0)
            {
                result = _inactiveObjects.Pop();
                if (result != null)
                {
                    break;
                }
                Debug.LogWarning($"Pooled item '{typeof(T).FullName}' was destroyed in outer scope.");
            }

            if (!result)
            {
                result = Object.Instantiate(_prefab, _parent, worldPositionStays: false);
            }

            if (_activateOnGet)
            {
                result.gameObject.SetActive(true);
            }

            _actionOnGet?.Invoke(result);
            return result;
        }

        public void Return(T item)
        {
            //Assertion.IsNull()
            if (!item)
            {
                Debug.LogWarning($"Can't return destroyed item '{typeof(T).FullName}' to the pool");
                return;
            }

            item.gameObject.SetActive(false);
            _actionOnReturn?.Invoke(item);
            _inactiveObjects.Push(item);
        }

        public void Dispose()
        {
            while (_inactiveObjects.Count > 0)
            {
                Object.Destroy(_inactiveObjects.Pop());
            }
        }
    }
}