using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        protected override UniTask Hide(float duration)
        {
            return AnimateScale(Vector3.zero, duration);
        }

        protected override UniTask Show(float duration)
        {
            return AnimateScale(Vector3.one, duration);
        }

        public override void ShowImmediately()
        {
            _block.localScale = Vector3.one;
        }

        public override void HideImmediately()
        {
            _block.localScale = Vector3.zero;
        }

        private UniTask AnimateScale(Vector3 targetScale, float duration)
        {
            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();

            _block.DOKill();
            _block.DOScale(targetScale, duration)
                .SetEase(Ease.OutQuint)
                .SetUpdate(UpdateType.Late, true)
                .SetLink(gameObject)
                .OnComplete(() => completionSource.TrySetResult())
                .OnKill(() => completionSource.TrySetResult());

            return completionSource.Task;
        }
    }
}
