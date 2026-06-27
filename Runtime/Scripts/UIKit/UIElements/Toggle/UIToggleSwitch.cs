using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("ToggleSwitch")]
    public class UIToggleSwitch : ToggleSwitch
    {
        [Header("Components")]
        public RectTransform Handler;
        public Image Image;

        [Header("Animation Settings")]
        public float AnimateTime = 0.2f;
        public Ease AnimateType = Ease.Linear;
        public Color ColorOn = Color.green;
        public Color ColorOff = Color.gray;
        public float AncorMinX = 5f;
        public float AncorMaxX = -5f;

        protected override void OnSwicthed(bool isOn)
        {
            Image.DOKill();
            Handler.DOKill();

            Handler.DOAnchorMin(new Vector2(isOn ? 1 : 0, 0.5f), AnimateTime).SetEase(AnimateType);
            Handler.DOAnchorMax(new Vector2(isOn ? 1 : 0, 0.5f), AnimateTime).SetEase(AnimateType);

            Handler.DOPivotX(isOn ? 1 : 0, AnimateTime).SetEase(AnimateType);

            Image.DOColor(isOn ? ColorOn : ColorOff, AnimateTime).SetEase(AnimateType);
            Handler.DOAnchorPosX(isOn ? AncorMaxX : AncorMinX, AnimateTime).SetEase(AnimateType);
        }

        protected override void OnSwicthedImmediately(bool isOn)
        {
            Image.DOKill();
            Handler.DOKill();

            Handler.anchorMax = new Vector2(isOn ? 1 : 0, 0.5f);
            Handler.anchorMin = new Vector2(isOn ? 1 : 0, 0.5f);

            Handler.pivot = new Vector2(isOn ? 1 : 0, 0.5f);

            Image.color = isOn ? ColorOn : ColorOff;
            Handler.anchoredPosition =
                new Vector2(isOn ? AncorMaxX : AncorMinX, Handler.anchoredPosition.y);
        }
    }
}
