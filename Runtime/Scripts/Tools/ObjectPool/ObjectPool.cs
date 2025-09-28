using System;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject<T>
    {
        public int PoolSize;
        public int PoolIncreaseStep;

        protected Queue<T> _pool;
        protected HashSet<T> _allPoolObjects;
        protected T _objectPrefab;
        protected int _currentPoolSize;
        protected Transform _poolParent;

        public ObjectPool(T prefab, Transform parent = null, bool isFillAtStart = true, int startPoolSize = 10, int poolIncreaseStep = 10)
        {
            PoolSize = startPoolSize;
            PoolIncreaseStep = poolIncreaseStep;
            _objectPrefab = prefab;
            _allPoolObjects = new HashSet<T>();
            _poolParent = parent;
            _pool = new Queue<T>();
            if(isFillAtStart)
            {
                FillPool();
            }
            else
            {
                PoolSize = 0;
            }
        }

        public T Get()
        {
            if (_pool.Count == 0)
                ExpandPool();

            T obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            obj.OnActive();
            return obj;
        }

        protected virtual void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }

        protected void ExpandPool()
        {
            PoolSize += PoolIncreaseStep;
            FillPool();
        }

        protected void FillPool()
        {
            while (_currentPoolSize < PoolSize)
            {
                CreataAndAdd();
            }
        }

        private  void CreataAndAdd()
        {
            T gameObject = Create();
            gameObject.OnBackToPool += Release;
            _allPoolObjects.Add(gameObject);
            Release(gameObject);
            _currentPoolSize++;
        }

        protected virtual T Create() 
            => UnityEngine.Object.Instantiate(_objectPrefab, _poolParent);

        public void Dispose(bool destroyObjects = true)
        {
            foreach (var obj in _allPoolObjects)
            {
                if (obj != null)
                    obj.OnBackToPool -= Release;
                if (destroyObjects)
                    UnityEngine.Object.Destroy(obj.gameObject);
            }

            _pool.Clear();
            _allPoolObjects.Clear();
            _currentPoolSize = 0;
        }
    }
}
