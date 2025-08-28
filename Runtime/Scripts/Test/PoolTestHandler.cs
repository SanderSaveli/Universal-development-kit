using SanderSaveli.UDK.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SanderSaveli.UDK
{
    public class PoolTestHandler : MonoBehaviour
    {
        [SerializeField] private PoolTestObj _prefab;
        [SerializeField] private Button _button;
        [SerializeField] private Transform _parent;
        private InjectionObjectPool<PoolTestObj> pool;
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Start()
        {
            pool = new InjectionObjectPool<PoolTestObj>(_container, _prefab, _parent);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Get);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Get);
        }

        private void Get()
        {
            pool.Get();
        }
    }
}
