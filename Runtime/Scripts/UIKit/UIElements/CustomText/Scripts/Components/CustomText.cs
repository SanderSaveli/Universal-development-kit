using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CustomText
{
    [AddComponentMenu("UI/Custom Components/Custom Text")]
    public class CustomText : TextMeshProUGUI
    {
        [SerializeField] private Custom_TextStyle _textStyle = Custom_TextStyle.Default;
        [SerializeField] private Custom_ColorStyle _textColor = Custom_ColorStyle.Default;
        [SerializeField] private Custom_MaterialStyle _textMaterial = Custom_MaterialStyle.Default;

        private Custom_TextStyle _selectedTextStyle = Custom_TextStyle.Default;
        private Custom_ColorStyle _selectedTextColor = Custom_ColorStyle.Default;
        private Custom_MaterialStyle _selectedTextMaterial = Custom_MaterialStyle.Default;

        private bool _isSubcribed = false;

        protected override void Start()
        {
            Init();
        }

        public void ChangeStyle(Custom_TextStyle textStyle)
        {
            _textStyle = textStyle;
            ApplyTextStyleSetting();
        }

        public void ChangeColor(Custom_ColorStyle textColor)
        {
            _textColor = textColor;
            ApplyColorSetting();
        }

        public void ChangeColorWithAnimation(Custom_ColorStyle textColor, float duration = 0.4f)
        {
            _textColor = textColor;
            _selectedTextColor = _textColor;
            Color newColor = ColorSettings.Instance.GetColorByStyle(_textColor);
            this.DOColor(newColor, duration).SetLink(gameObject);
        }

        public void ChangeMaterial(Custom_MaterialStyle textMaterial)
        {
            _textMaterial = textMaterial;
            ApplyMaterialSetting();
        }

        private void Init()
        {
            ApplyTextStyleSetting();
            ApplyColorSetting();
            ApplyMaterialSetting();
        }

        private void ApplyTextStyleSetting()
        {
            _selectedTextStyle = _textStyle;

            if (TextStyleSettings.Instance == null) return;
            var textTypeSettings = TextStyleSettings.Instance.GetSettingsTyStyle(_textStyle);

            if (textTypeSettings != null)
            {
                fontSize = textTypeSettings.TextSize;
                m_fontAsset = textTypeSettings.Font;

                fontWeight = textTypeSettings.FontWeight;
                characterSpacing = textTypeSettings.CharacterSpacing;
                lineSpacing = textTypeSettings.LineSpacing;
            }
        }

        private void ApplyColorSetting()
        {
            _selectedTextColor = _textColor;
            if (ColorSettings.Instance == null) return;

            Color textColor = ColorSettings.Instance.GetColorByStyle(_textColor);
            color = textColor;
        }

        private void ApplyMaterialSetting()
        {
            _selectedTextMaterial = _textMaterial;
            if (MaterialSettings.Instance == null) return;
            var material = MaterialSettings.Instance.Materials.Find(t => t.MaterialType.Equals(_textMaterial));
            if (material != null)
            {
                fontSharedMaterial = material.Material;
            }
        }

        protected override void OnDestroy()
        {
            ColorSettings.Instance.OnColorStyleChanged -= ApplyColorSetting;
            TextStyleSettings.Instance.OnTextStyleChanged -= ApplyTextStyleSetting;
            MaterialSettings.Instance.OnMaterialStyleChanged -= ApplyMaterialSetting;
            _isSubcribed = false;
            base.OnDestroy();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!_isSubcribed)
            {
                ColorSettings.Instance.OnColorStyleChanged += ApplyColorSetting;
                TextStyleSettings.Instance.OnTextStyleChanged += ApplyTextStyleSetting;
                MaterialSettings.Instance.OnMaterialStyleChanged += ApplyMaterialSetting;
                _isSubcribed = true;
            }

            if (_textStyle != _selectedTextStyle) ApplyTextStyleSetting();
            if (_textColor != _selectedTextColor) ApplyColorSetting();
            if (_textMaterial != _selectedTextMaterial) ApplyMaterialSetting();

            base.OnValidate();
        }
#endif
    }
}
