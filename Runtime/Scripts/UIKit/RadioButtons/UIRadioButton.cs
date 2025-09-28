using CustomText;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public class UIRadioButton<T> : RadioButton<T> where T :Enum
    {
        [SerializeField] public ImageColorByType ImageColor;
        [SerializeField] public CustomText.CustomText Text;
        [SerializeField] public Button Button;

        [Header("Image")]
        [SerializeField] public Custom_ColorStyle SelectedImageColor;
        [SerializeField] public Custom_ColorStyle DeselectedImageColor;
        [Header("Text")]
        [SerializeField] public Custom_ColorStyle SelectedTextColor;
        [SerializeField] public Custom_ColorStyle DeselectedTexxtColor;

        private void OnEnable()
        {
            Button.onClick.AddListener(InputSelect);
        }
        private void OnDisable()
        {
            Button.onClick.RemoveListener(InputSelect);
        }
        public override void Select()
        {
            ImageColor.ChangeColorWithAnimation(SelectedImageColor);
            Text.ChangeColor(SelectedTextColor);
        }

        public override void Deselect()
        {
            ImageColor.ChangeColorWithAnimation(DeselectedImageColor);
            Text.ChangeColor(DeselectedTexxtColor);
        }

        private void InputSelect()
        {
            OnSelectInput?.Invoke(this);
        }
    }
}
