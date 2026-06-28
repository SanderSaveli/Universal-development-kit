using CustomText;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    [AddComponentMenu("UI/Custom Components/Image Color By Type")]
    [RequireComponent(typeof(Image))]
    public class ImageColorByType : ColorByType
    {
        [SerializeField] private Image _image;

        private void Reset()
        {
            _image = GetComponent<Image>();
        }

        protected override void OnDestroy()
        {
            DOTween.Kill(_image);
            base.OnDestroy();
        }

        protected override void ApplyColorWithAnimation(Color color, float duration)
        {
            _image.DOColor(color, duration).SetLink(gameObject);
        }

        protected override void ApplyColor(Color color) => _image.color = color;
        protected override float GetCurrentColorAlpha() => _image.color.a;
    }
}