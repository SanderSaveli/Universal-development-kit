using DG.Tweening;
using TMPro;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class ColorChangeTextAnimation : TextAnimation
    {
        [Space]
        [Range(0f, 1f)]
        [SerializeField] private float _colorBlickTimePosition = 0.5f;
        [SerializeField] private float _highlightDuration = 0f;

        [Space]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _highlightColor = Color.gray;

        private Sequence _sequence;

        protected override void AnimateText(string text, TMP_Text field, float duration)
        {
            float highLightDuration = duration * _colorBlickTimePosition;
            float backToNormalDuration = Mathf.Max(duration - (highLightDuration + _highlightDuration), 0, 001);
            field.text = text;

            if(_sequence != null)
            {
                _sequence.Kill();
            }
            _sequence = DOTween.Sequence();
            _sequence
                .Append(field.DOColor(_highlightColor, highLightDuration))
                .AppendInterval(_highlightDuration)
                .Append(field.DOColor(_normalColor, backToNormalDuration))
                .SetLink(gameObject)
                .OnComplete(() => _sequence = null);
        }
    }
}
