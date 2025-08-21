using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK.UI
{
    public class ItemFiller<TSlot, T> : MonoBehaviour where TSlot : MonoBehaviour, ISlot<T>
    {
        [SerializeField] protected Transform _contentPatent;
        [Header("Prefab")]
        [SerializeField] protected TSlot _content;

        private DiContainer _container;
        protected List<TSlot> _slots = new List<TSlot>();

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        public virtual void FillItems(List<T> items)
        {
            int i = 0;
            foreach (T item in items)
            {
                TSlot slot;

                if (_slots.Count > i)
                {
                    slot = _slots[i];
                }
                else
                {
                    slot = _container
                        .InstantiatePrefabForComponent<TSlot>(_content, _contentPatent);
                    _slots.Add(slot);
                }

                slot.Fill(item);
                SlotCreated(slot);
                i++;
            }

            for (; i < _contentPatent.childCount; i++)
            {
                TSlot destroySlot = _slots[i];
                _slots.RemoveAt(i);
                DestroyImmediate(destroySlot.gameObject);
                i--;
            }
        }

        public virtual void SlotCreated(TSlot slot)
        {

        }
    }
}
