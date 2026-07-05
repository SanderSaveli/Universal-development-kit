using Cysharp.Threading.Tasks;
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

        public async UniTask Show()
        {
            await Show(_showDuration, _showDelay);
        }

        public async UniTask Hide()
        {
            await Hide(_hideDuration, _hideDelay);
        }

        public async UniTask Show(float duration, float delay = 0)
        {
            await UniTask.WaitForSeconds(delay);
            await Show(duration);
        }

        public async UniTask Hide(float duration, float delay = 0)
        {
            await UniTask.WaitForSeconds(delay);
            await Hide(duration);
        }

        public abstract void ShowImmediately();
        public abstract void HideImmediately();

        protected abstract UniTask Show(float duration);
        protected abstract UniTask Hide(float duration);
    }
}
