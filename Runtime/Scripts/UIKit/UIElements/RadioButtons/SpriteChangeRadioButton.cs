using CustomText;
using SanderSaveli.UDK.UI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SanderSaveli.UDK
{
    public class SpriteChangeRadioButton<T> : UiRadioButton<T> where T : Enum
    {
        [SerializeField] public Image Image;
        [SerializeField] public CustomText.CustomText Text;

        [Header("Image")]
        [SerializeField] public Sprite SelectedImageSprite;
        [SerializeField] public Sprite DeselectedImageSprite;
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
        protected override void OnSelect()
        {
            Image.sprite = SelectedImageSprite;
            Text.ChangeColor(SelectedTextColor);
        }

        protected override void OnDeselect()
        {
            Image.sprite = DeselectedImageSprite;
            Text.ChangeColor(DeselectedTexxtColor);
        }
    }
}
