using CustomText;
using SanderSaveli.UDK.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class ColorChangeDisableableButton : DisableableButton
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
