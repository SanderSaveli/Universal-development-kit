using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UiScreen : MonoBehaviour
    {
        public Action<bool> OnScreenStatusChange { get; set; }

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

        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);
            ScreenRect.DOKill();
            if (Background != null)
            {
                Background.Show().Forget();
            }

            OnScreenStatusChange?.Invoke(true);
            await UiScreenAnimator.Show();
        }
        public virtual void ShowImmediately()
        {
            gameObject.SetActive(true);
            UiScreenAnimator.ShowImmediately();
            Background?.ShowImmediately();
        }

        public virtual async UniTask Hide(Action callback = null)
        {
            ScreenRect.DOKill();
            if (Background != null)
            {
                Background.Hide().Forget();
            }

            await UiScreenAnimator.Hide();
            callback?.Invoke();
            OnScreenStatusChange?.Invoke(false);
            gameObject.SetActive(false);
        }

        public virtual void HideImmediately()
        {
            UiScreenAnimator.HideImmediately();
            Background?.HideImmediately();
            gameObject.SetActive(false);
        }

        protected virtual void OnDestroy()
        {
            ScreenRect.DOKill();
        }

        protected virtual void SubscribeToEvents()
        { }

        protected virtual void UnsubscribeFromEvents()
        { }
    }
}
