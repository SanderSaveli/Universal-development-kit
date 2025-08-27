using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public abstract class RadioButtonGroup<T> : MonoBehaviour
    {
        [SerializeField] private int _startSelectedElement;

        [SerializeField] private List<RadioButton<T>> _radioButtons;

        public T Value
        {
            get
            {
                if (_selectedElement != null && !Equals(_selectedElement.Value, default(T)))
                    return _selectedElement.Value;

                return _radioButtons[_startSelectedElement].Value;
            }
        }

        public Action<T> OnValueChanged;

        private RadioButton<T> _selectedElement;

        private void Awake()
        {
            _selectedElement = _radioButtons[_startSelectedElement];
        }

        private void Start()
        {
            foreach (var radioButton in _radioButtons)
            {
                radioButton.Deselect();
                radioButton.OnSelectInput += OnSelectInput;
            }
            _selectedElement.Select();
        }

        public void SetSelect(T type)
        {
            RadioButton<T> button = _radioButtons.FirstOrDefault(t => t.Value.Equals(type));
            OnSelectInput(button);
        }

        protected void OnSelectInput(RadioButton<T> radioButton)
        {
            if (radioButton == _selectedElement)
            {
                Debug.Log("this one");
                return;
            }

            _selectedElement.Deselect();
            _selectedElement = radioButton;
            radioButton.Select();
            OnValueChanged?.Invoke(Value);
        }

        protected void OnValidate()
        {
            if (_radioButtons != null)
            {
                _startSelectedElement = Mathf.Clamp(_startSelectedElement, 0, _radioButtons.Count - 1);
            }
        }
    }
}
