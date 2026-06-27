using System;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public abstract class RadioButton<TValue> : MonoBehaviour, ISelectable
    {
        public TValue Value => _value;
        public bool IsSelected { get; private set; }
        public Action<RadioButton<TValue>> OnSelectInput { get; set; }
        public Action<bool> OnSelectChange { get; set; }

        [SerializeField] private TValue _value;

        public void Select()
        {
            IsSelected = true;
            OnSelectChange?.Invoke(IsSelected);
            OnSelect();
        }

        public void Deselect()
        {
            IsSelected = false;
            OnSelectChange?.Invoke(IsSelected);
            OnDeselect();
        }

        protected abstract void OnSelect();
        protected abstract void OnDeselect();
    }
}
