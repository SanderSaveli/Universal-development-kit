using SanderSaveli.UDK.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK
{
    public abstract class ToggleSwitch : MonoBehaviour, ISelectable
    {
        public bool IsSelected => _isSelected;
        public Action<bool> OnSelectChange { get; set; }

        [Header("General")]
        [SerializeField] protected Button _button;
        [SerializeField] protected bool _isOnAtStart;

        private bool _isSelected;

        private void Reset()
        {
            _button = GetComponent<Button>();
        }

        private void Awake()
        {
            SetValueImmediately(_isOnAtStart);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Toggle);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Toggle);
        }
        public void Select() => SetValue(true);

        public void Deselect() => SetValue(false);

        public void SetValue(bool isOn)
        {
            _isSelected = isOn;
            OnSwicthed(isOn);

            OnSelectChange?.Invoke(isOn);
        }

        public void SetValueImmediately(bool isOn)
        {
            _isSelected = isOn;
            OnSwicthedImmediately(isOn);

            OnSelectChange?.Invoke(isOn);
        }

        protected abstract void OnSwicthed(bool isOn);
        protected abstract void OnSwicthedImmediately(bool isOn);
        private void Toggle() => SetValue(!_isSelected);
    }
}
