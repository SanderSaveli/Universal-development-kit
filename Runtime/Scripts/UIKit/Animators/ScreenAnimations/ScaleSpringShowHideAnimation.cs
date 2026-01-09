using DG.Tweening;
using System;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public class ScaleSpringShowHideAnimation : ShowHideAnimation
    {
        [SerializeField] private RectTransform _block;

        [Header("Spring Settings")]
        [Min(1)]
        [SerializeField] private float _overshootScale = 1.15f;
        [Range(0f, 1f)]
        [SerializeField] private float _overshotTimePosition = 0.8f;

        private void Reset()
        {
            _block = GetComponent<RectTransform>();
        }

        protected override void Show(float duration, Action callback)
        {
            _block.DOKill();

            _block.localScale = Vector3.zero;

            float overshootDuration = duration * _overshotTimePosition;
            float settleDuration = duration - overshootDuration;

            Sequence sequence = DOTween.Sequence();
            sequence.SetUpdate(UpdateType.Late, true);
            sequence.SetLink(gameObject);

            sequence.Append(
                _block.DOScale(Vector3.one * _overshootScale, overshootDuration)
                      .SetEase(Ease.OutBack)
            );

            sequence.Append(
                _block.DOScale(Vector3.one, settleDuration)
                      .SetEase(Ease.OutQuint)
            );

            sequence.OnComplete(() => callback?.Invoke());
        }

        protected override void Hide(float duration, Action callback)
        {
            _block.DOKill();

            _block.DOScale(Vector3.zero, duration)
                .SetEase(Ease.InBack)
                .SetUpdate(UpdateType.Late, true)
                .SetLink(gameObject)
                .OnComplete(() => callback?.Invoke());
        }

        public override void ShowImmediately()
        {
            _block.DOKill();
            _block.localScale = Vector3.one;
        }

        public override void HideImmediately()
        {
            _block.DOKill();
            _block.localScale = Vector3.zero;
        }
    }
}
