using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class ObjectPool<T> where T : MonoBehaviour, IPoolableObject<T>
    {
        public int PoolSize;
        public int PoolIncreaseStep;

        public Queue<T> Pool;
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
            Pool = new Queue<T>();
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
            if (Pool.Count == 0)
                ExpandPool();

            T obj = Pool.Dequeue();
            obj.gameObject.SetActive(true);
            obj.OnActive();
            return obj;
        }

        protected virtual void Release(T obj)
        {
            obj.gameObject.SetActive(false);
            Pool.Enqueue(obj);
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
            => Object.Instantiate(_objectPrefab, _poolParent);

        public void Dispose(bool destroyObjects = true)
        {
            foreach (var obj in _allPoolObjects)
            {
                if (obj != null)
                    obj.OnBackToPool -= Release;
                if (destroyObjects)
                    Object.Destroy(obj.gameObject);
            }

            Pool.Clear();
            _allPoolObjects.Clear();
            _currentPoolSize = 0;
        }
    }
}
