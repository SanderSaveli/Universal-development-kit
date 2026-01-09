using DG.Tweening;
using TMPro;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class ScaleTextAnimation : TextAnimation
    {
        [Space]
        [Range(0f, 1f)]
        [SerializeField] private float _maxScaleTimePosition = 0.5f;
        [SerializeField] private float _maxScaleDuration = 0f;
        [Min(0)]
        [SerializeField] private float _maxScale = 1.2f;
        [SerializeField] private float _normalScale = 1;

        [Space]
        [SerializeField] private Ease _inEase = Ease.OutQuad;
        [SerializeField] private Ease _outEase = Ease.OutQuad;

        private Sequence _sequence;

        protected override void AnimateText(string text, TMP_Text field, float duration)
        {
            float toMaxScaleDuration = duration * _maxScaleTimePosition;
            float backToNormalDuration = Mathf.Max(duration - (toMaxScaleDuration + _maxScaleDuration), 0, 001);
            field.text = text;

            if (_sequence != null)
            {
                _sequence.Kill();
            }
            _sequence = DOTween.Sequence();
            _sequence
                .Append(field.transform.DOScale(_maxScale, toMaxScaleDuration))
                .SetEase(_inEase)
                .AppendInterval(_maxScaleDuration)
                .Append(field.transform.DOScale(_normalScale, backToNormalDuration))
                .SetEase(_outEase)
                .SetLink(gameObject)
                .OnComplete(() => _sequence = null);
        }
    }
}
