using System;
using System.Collections;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public abstract class ShowHideAnimation : MonoBehaviour
    {
        [Header("Show")]
        [Min(0)]
        [SerializeField] private float _showDuration = 0.5f;
        [Min(0)]
        [SerializeField] private float _showDelay = 0f;

        [Header("Hide")]
        [Min(0)]
        [SerializeField] private float _hideDuration = 0.5f;
        [Min(0)]
        [SerializeField] private float _hideDelay = 0f;

        public void Show(Action callback = null)
        {
            Show(_showDuration, _showDelay, callback);
        }

        public void Hide(Action callback = null)
        {
            Hide(_hideDuration, _hideDelay, callback);
        }

        public void Show(float duration, float delay, Action callback = null)
        {
            StartCoroutine(Delay(delay, () => Show(duration, callback)));
        }

        public void Hide(float duration, float delay, Action callback = null)
        {
            StartCoroutine(Delay(delay, () => Hide(duration, callback)));
        }

        public abstract void ShowImmediately();
        public abstract void HideImmediately();

        protected abstract void Show(float duration, Action callback);
        protected abstract void Hide(float duration, Action callback);

        private IEnumerator Delay(float delay, Action callback)
        {
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            callback.Invoke();
        }
    }
}
