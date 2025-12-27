using DG.Tweening;
using SanderSaveli.UDK.UI;
using System;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public enum SlideDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    [RequireComponent(typeof(RectTransform))]
    public class SlideShowHideAnimation : ShowHideAnimation
    {
        [Header("Show")]
        [SerializeField] private SlideDirection _enterFrom = SlideDirection.Left;
        [SerializeField] private Ease _enterEase = Ease.OutCubic;
        [Header("Hide")]
        [SerializeField] private SlideDirection _exitTo = SlideDirection.Right;
        [SerializeField] private Ease _exitEase = Ease.InCubic;

        [Space]
        [SerializeField] private float offsetMultiplier = 1.2f;

        private RectTransform _rectTransform;
        private Vector2 _initialAnchoredPosition;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _initialAnchoredPosition = _rectTransform.anchoredPosition;
            transform.localScale = Vector3.one;
        }

        public override void Hide(float delay, float duration, Action callback)
        {
            Vector2 toPos = GetOffsetPosition(_exitTo);

            Animate(toPos, duration, delay, _exitEase, callback);
        }

        public override void HideImmediately()
        {
            Vector2 toPos = GetOffsetPosition(_exitTo);
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = toPos;
        }

        public override void Show(float delay, float duration, Action callback)
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            Vector2 fromPos = GetOffsetPosition(_enterFrom);
            _rectTransform.anchoredPosition = fromPos;

            Animate(_initialAnchoredPosition, duration, delay, _enterEase, callback);
        }

        public override void ShowImmediately()
        {
            _rectTransform.anchoredPosition = _initialAnchoredPosition;
        }

        private void Animate(Vector2 anchoredPosition, float duration, float delay, Ease ease, Action callback)
        {
            _rectTransform.DOAnchorPos(anchoredPosition, duration)
                .SetEase(ease)
                .SetDelay(delay)
                .SetUpdate(true)
                .OnComplete(() => callback?.Invoke())
                .SetLink(gameObject);
        }

        private Vector2 GetOffsetPosition(SlideDirection direction)
        {
            Vector2 offset = Vector2.zero;
            Vector2 canvasSize = GetCanvasSize();

            switch (direction)
            {
                case SlideDirection.Left:
                    offset = new Vector2(-canvasSize.x * offsetMultiplier, _initialAnchoredPosition.y);
                    break;
                case SlideDirection.Right:
                    offset = new Vector2(canvasSize.x * offsetMultiplier, _initialAnchoredPosition.y);
                    break;
                case SlideDirection.Top:
                    offset = new Vector2(_initialAnchoredPosition.x, canvasSize.y * offsetMultiplier);
                    break;
                case SlideDirection.Bottom:
                    offset = new Vector2(_initialAnchoredPosition.x, -canvasSize.y * offsetMultiplier);
                    break;
            }

            return offset;
        }

        private Vector2 GetCanvasSize()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.pixelRect != null)
            {
                return canvas.pixelRect.size;
            }
            return new Vector2(Screen.width, Screen.height);
        }
    }
}
