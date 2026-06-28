using CustomText;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public abstract class ColorByType : MonoBehaviour
    {
        public Custom_ColorStyle Color => _color;

        [SerializeField] protected Custom_ColorStyle _color;
        [SerializeField] protected bool _isOverrideAlpha = true;

        private bool _isSubcribed = false;
        private Custom_ColorStyle _selectedColor = Custom_ColorStyle.Default;

        private void Awake() => ApplyColorSetting();

        public void ChangeColor(Custom_ColorStyle style)
        {
            _selectedColor = style;
            if (ColorSettings.Instance == null) return;

            _color = _selectedColor;
            Change();
        }

        public void ChangeColorWithAnimation(Custom_ColorStyle style, float duration = 0.4f)
        {
            _selectedColor = style;

            if (ColorSettings.Instance == null) return;

            Color color = GetColor(style);
            _color = _selectedColor;
            Change();
            ApplyColorWithAnimation(color, duration);
        }

        protected abstract void ApplyColorWithAnimation(Color color, float duration);
        protected abstract void ApplyColor(Color color);
        protected abstract float GetCurrentColorAlpha();
        private void ApplyColorSetting()
        {
            _selectedColor = _color;
            if (ColorSettings.Instance == null) return;
            Color color = GetColor(_selectedColor);
            ApplyColor(color);
        }

        protected Color GetColor(Custom_ColorStyle style)
        {
            Color color = ColorSettings.Instance.GetColorByStyle(_selectedColor);
            if (!_isOverrideAlpha)
            {
                float currentAlpha = GetCurrentColorAlpha();
                color.a = currentAlpha;
            }
            return color;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Change();
        }
#endif

        private void Change()
        {
            if (!_isSubcribed)
            {
                ColorSettings.Instance.OnColorStyleChanged += ApplyColorSetting;
                _isSubcribed = true;
            }

            if (_color != _selectedColor) ApplyColorSetting();
        }

        protected virtual void OnDestroy()
        {
            ColorSettings.Instance.OnColorStyleChanged -= ApplyColorSetting;
        }
    }
}
