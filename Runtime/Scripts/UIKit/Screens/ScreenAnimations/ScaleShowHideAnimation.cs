using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public class ScaleShowHideAnimation : ShowHideAnimation
    {
        [SerializeField] private RectTransform _screen;

        private void Reset()
        {
            _screen = gameObject.GetComponent<RectTransform>();
        }

        public override void Hide(float delay, float duration, Action callback)
        {
            AnimateScale(Vector3.zero, delay, duration, callback);
        }

        public override void Show(float delay, float duration, Action callback)
        {
            AnimateScale(Vector3.one, delay, duration, callback);
        }

        public override void ShowImmediately()
        {
            _screen.localScale = Vector3.one;
        }
        public override void HideImmediately()
        {
            _screen.localScale = Vector3.zero;
        }

        private void AnimateScale(Vector3 targetScale,
            float delay,
            float duration,
            Action callback = null
        )
        {
            _screen.DOKill();
            _screen.DOScale(targetScale, duration)
                .SetEase(Ease.OutQuint)
                .SetUpdate(UpdateType.Late, true)
                .SetDelay(delay)
                .OnUpdate(() =>
                {
                    List<RectTransform> rectTransforms = _screen.gameObject.GetComponentsInChildren<RectTransform>().ToList();
                    rectTransforms.ForEach(LayoutRebuilder.ForceRebuildLayoutImmediate);
                })
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }
    }
}