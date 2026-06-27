using SanderSaveli.UDK.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK
{
    [RequireComponent(typeof(Button))]
    public abstract class DisableableButton : MonoBehaviour, ISelectable
    {
        public bool IsSelected { get; private set; }
        public Action<bool> OnSelectChange { get; set; }
        [Header("Components")]
        [SerializeField] private Button _button;

        private void Reset()
        {
            _button = GetComponent<Button>();
        }
        public void Deselect()
        {
            IsSelected = false;
            _button.interactable = false;
            OnDeselect();
            OnSelectChange?.Invoke(IsSelected);
        }

        public void Select()
        {
            IsSelected = true;
            _button.interactable = true;
            OnSelect();
            OnSelectChange?.Invoke(IsSelected);
        }

        protected abstract void OnSelect();

        protected abstract void OnDeselect();
    }
}
