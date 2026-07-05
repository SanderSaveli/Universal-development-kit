using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
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

        protected override UniTask Show(float duration)
        {
            return AnimateFade(_targetMaxValue, true, duration);
        }

        protected override UniTask Hide(float duration)
        {
            return AnimateFade(0f, false, duration);
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

        private UniTask AnimateFade(float targetAlpha, bool statusAfter, float duration)
        {
            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();

            _canvasGroup.transform.localScale = Vector3.one;
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(targetAlpha, duration)
                .SetEase(Ease.OutQuint)
                .SetUpdate(UpdateType.Late, true)
                .OnComplete(() =>
                {
                    InterractCanvas(statusAfter);
                    completionSource.TrySetResult();
                })
                .OnKill(() => completionSource.TrySetResult());

            return completionSource.Task;
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
