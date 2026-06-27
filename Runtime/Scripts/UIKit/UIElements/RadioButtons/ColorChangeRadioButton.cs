using CustomText;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public class ColorChangeRadioButton<T> : UiRadioButton<T> where T : Enum
    {
        [SerializeField] public ImageColorByType ImageColor;
        [SerializeField] public CustomText.CustomText Text;

        [Header("Image")]
        [SerializeField] public Custom_ColorStyle SelectedImageColor;
        [SerializeField] public Custom_ColorStyle DeselectedImageColor;
        [Header("Text")]
        [SerializeField] public Custom_ColorStyle SelectedTextColor;
        [SerializeField] public Custom_ColorStyle DeselectedTexxtColor;

        protected override void OnSelect()
        {
            ImageColor.ChangeColorWithAnimation(SelectedImageColor);
            Text.ChangeColor(SelectedTextColor);
        }

        protected override void OnDeselect()
        {
            ImageColor.ChangeColorWithAnimation(DeselectedImageColor);
            Text.ChangeColor(DeselectedTexxtColor);
        }
    }
}
