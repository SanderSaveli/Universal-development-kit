using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK.Tools
{
    public class InjectionObjectPool<T> : ObjectPool<T> where T : MonoBehaviour, IPoolableObject<T>
    {
        private DiContainer _container;

        public InjectionObjectPool(DiContainer container, T prefab, Transform parent = null, int startPoolSize = 10, int poolIncreaseStep = 10) : base(prefab, parent, false, startPoolSize, poolIncreaseStep)
        {
            _container = container;
            ExpandPool();
        }

        protected override T Create() =>
            _container.InstantiatePrefabForComponent<T>(_objectPrefab, _poolParent);
    }
}
