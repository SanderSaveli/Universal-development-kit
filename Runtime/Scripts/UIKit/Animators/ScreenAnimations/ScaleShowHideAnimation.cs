using DG.Tweening;
using System;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public class ScaleShowHideAnimation : ShowHideAnimation
    {
        [SerializeField] private RectTransform _block;

        private void Reset()
        {
            _block = gameObject.GetComponent<RectTransform>();
        }

        protected override void Hide(float duration, Action callback)
        {
            AnimateScale(Vector3.zero, duration, callback);
        }

        protected override void Show(float duration, Action callback)
        {
            AnimateScale(Vector3.one, duration, callback);
        }

        public override void ShowImmediately()
        {
            _block.localScale = Vector3.one;
        }
        public override void HideImmediately()
        {
            _block.localScale = Vector3.zero;
        }

        private void AnimateScale(Vector3 targetScale,
            float duration,
            Action callback = null
        )
        {
            Debug.Log(targetScale);
            _block.DOKill();
            _block.DOScale(targetScale, duration)
                .SetEase(Ease.OutQuint)
                .SetUpdate(UpdateType.Late, true)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }
    }
}