using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public class BlocksShowHideAnimator : ShowHideAnimation
    {
        [Header("Blocks")]
        [SerializeField] private List<ShowHideAnimation> _blocks;

        private UniTask _animationTask;

        public override void ShowImmediately()
        {
            StopCurrentRoutine();

            foreach (ShowHideAnimation block in _blocks)
            {
                block.ShowImmediately();
            }
        }

        public override void HideImmediately()
        {
            StopCurrentRoutine();

            foreach (ShowHideAnimation block in _blocks)
            {
                block.HideImmediately();
            }
        }

        protected override async UniTask Show(float duration)
        {
            StopCurrentRoutine();
            _animationTask = AnimateRoutine(true);
            await _animationTask;
        }

        protected override async UniTask Hide(float duration)
        {
            StopCurrentRoutine();
            _animationTask = AnimateRoutine(false);
            await _animationTask;
        }

        private async UniTask AnimateRoutine(bool isShow)
        {
            List<UniTask> showTasks = new List<UniTask>();  

            foreach (ShowHideAnimation block in _blocks)
            {
                UniTask task = isShow ? block.Show() : block.Hide();
                showTasks.Add(task);
            }

            await UniTask.WhenAll(showTasks);

        }

        private void StopCurrentRoutine()
        {

        }
    }
}
