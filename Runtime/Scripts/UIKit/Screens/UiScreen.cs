using DG.Tweening;
using System;
using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UiScreen : MonoBehaviour
    {
        [SerializeField] protected float ShowTime = 0.5f;
        [SerializeField] protected float DelayTime;
        [SerializeField] protected bool IsDisableWhileHidden = true;
        [SerializeField] protected UiScreen Background;
        [SerializeField] protected ShowHideAnimation UiScreenAnimator;

        protected RectTransform ScreenRect;

        protected virtual void Reset()
        {
            if (GetComponent<ShowHideAnimation>() == null)
            {
                UiScreenAnimator = gameObject.AddComponent<ScaleShowHideAnimation>();
            }
        }

        protected virtual void Awake()
        {
            ScreenRect = GetComponent<RectTransform>();
            UiScreenAnimator.HideImmediately();
        }

        protected void OnEnable() => SubscribeToEvents();

        protected void OnDisable() => UnsubscribeFromEvents();

        public virtual void Show(Action callback = null)
        {
            if (IsDisableWhileHidden) gameObject.SetActive(true);
            ScreenRect.DOKill();
            UiScreenAnimator.Show(DelayTime, ShowTime, () => OnShow(callback));
            Background?.Show();
        }
        public virtual void ShowImmediately()
        {
            UiScreenAnimator.ShowImmediately();
            Background?.ShowImmediately();
        }

        public virtual void Hide(Action callback = null)
        {
            ScreenRect.DOKill();
            UiScreenAnimator.Hide(DelayTime, ShowTime, () => OnHide(callback));
            Background?.Hide();
        }

        public virtual void HideImmediately()
        {
            UiScreenAnimator.HideImmediately();
            Background?.HideImmediately();
        }

        protected virtual void OnDestroy()
        {
            ScreenRect.DOKill();
        }

        protected virtual void SubscribeToEvents()
        { }

        protected virtual void UnsubscribeFromEvents()
        { }

        private void OnShow(Action callback)
        {
            callback?.Invoke();
        }

        private void OnHide(Action callback)
        {
            if (IsDisableWhileHidden) gameObject.SetActive(false);

            callback?.Invoke();
        }
    }
}