using System;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public abstract class UiRadioButton<T> : RadioButton<T> where T : Enum
    {
        [Header("Button")]
        [SerializeField] public Button Button;

        private void OnEnable()
        {
            Button.onClick.AddListener(InputSelect);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(InputSelect);
        }

        protected virtual void InputSelect()
        {
            OnSelectInput?.Invoke(this);
        }
    }
}
