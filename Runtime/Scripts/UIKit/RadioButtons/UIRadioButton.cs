using CustomText;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public class UIRadioButton<T> : RadioButton<T>
    {
        [SerializeField] private ImageColorByType _imageColor;
        [SerializeField] private CustomText.CustomText _text;
        [SerializeField] private Button _button;

        [Header("Image")]
        [SerializeField] private Custom_ColorStyle _selectedImageColor;
        [SerializeField] private Custom_ColorStyle _deselectedImageColor;
        [Header("Text")]
        [SerializeField] private Custom_ColorStyle _selectedTextColor;
        [SerializeField] private Custom_ColorStyle _deselectedTexxtColor;

        private void OnEnable()
        {
            _button.onClick.AddListener(InputSelect);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveListener(InputSelect);
        }
        public override void Select()
        {
            _imageColor.ChangeColorWithAnimation(_selectedImageColor);
            _text.ChangeColor(_selectedTextColor);
        }

        public override void Deselect()
        {
            _imageColor.ChangeColorWithAnimation(_deselectedImageColor);
            _text.ChangeColor(_deselectedTexxtColor);
        }

        private void InputSelect()
        {
            OnSelectInput?.Invoke(this);
        }
    }
}
