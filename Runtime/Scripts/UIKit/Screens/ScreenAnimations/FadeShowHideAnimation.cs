using System;
using DG.Tweening;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    [Serializable]
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeShowHideAnimation : ShowHideAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _targetMaxValue = 1;
        [SerializeField] private bool _needChangeStartCanvasParameters = true;

        private void OnValidate()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void AnimateFade(float targetAlpha, bool statusAfter, float delay, float duration, Action callback = null)
        {
            _canvasGroup.transform.localScale = Vector3.one;
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(targetAlpha, duration)
                .SetEase(Ease.OutQuint)
                .SetUpdate(UpdateType.Late, true)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                    InterractCanvas(statusAfter);
                });
        }

        public override void Show(float delay, float duration, Action callback)
        {
            AnimateFade(_targetMaxValue, true, delay, duration, callback);
        }

        public override void Hide(float delay, float duration, Action callback)
        {
            AnimateFade(0f, false, delay, duration, callback);
        }

        public override void ShowImmediately()
        {
            _canvasGroup.transform.localScale = Vector3.one;
            _canvasGroup.alpha = 1;
            InterractCanvas(true);
        }

        public override void HideImmediately()
        {
            _canvasGroup.transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0;
            InterractCanvas(false);
        }

        private void InterractCanvas(bool isOn)
        {
            if (_needChangeStartCanvasParameters)
            {
                _canvasGroup.interactable = isOn;
                _canvasGroup.blocksRaycasts = isOn;
            }
        }
    }
}
