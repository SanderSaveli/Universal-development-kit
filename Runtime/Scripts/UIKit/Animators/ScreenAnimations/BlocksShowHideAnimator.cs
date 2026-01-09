using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public class BlocksShowHideAnimator : ShowHideAnimation
    {
        [Header("Blocks")]
        [SerializeField] private float _delayBetweenBlocks;
        [SerializeField] private List<ShowHideAnimation> _blocks;

        private Coroutine _currentRoutine;
        private bool _isShowing;
        private bool _isHiding;

        public override void ShowImmediately()
        {
            StopCurrentRoutine();

            foreach (var block in _blocks)
            {
                block.ShowImmediately();
            }
        }

        public override void HideImmediately()
        {
            StopCurrentRoutine();

            foreach (var block in _blocks)
            {
                block.HideImmediately();
            }
        }

        protected override void Show(float duration, Action callback)
        {
            StopCurrentRoutine();
            _currentRoutine = StartCoroutine(ShowRoutine(callback));
        }

        protected override void Hide(float duration, Action callback)
        {
            StopCurrentRoutine();
            _currentRoutine = StartCoroutine(HideRoutine(callback));
        }

        private IEnumerator ShowRoutine(Action callback)
        {
            _isShowing = true;
            _isHiding = false;

            int completed = 0;
            int total = _blocks.Count;

            for (int i = 0; i < total; i++)
            {
                if (_isHiding) yield break;

                _blocks[i].Show(() => completed++);

                if (_delayBetweenBlocks > 0f)
                    yield return new WaitForSeconds(_delayBetweenBlocks);
            }

            yield return new WaitUntil(() => completed == total);

            _isShowing = false;
            callback?.Invoke();
        }


        private IEnumerator HideRoutine(Action callback)
        {
            _isHiding = true;
            _isShowing = false;

            int completed = 0;
            int total = _blocks.Count;

            for (int i = total - 1; i >= 0; i--)
            {
                if (_isShowing) yield break;

                _blocks[i].Hide(() => completed++);

                if (_delayBetweenBlocks > 0f)
                    yield return new WaitForSeconds(_delayBetweenBlocks);
            }

            yield return new WaitUntil(() => completed == total);

            _isHiding = false;
            callback?.Invoke();
        }


        private void StopCurrentRoutine()
        {
            if (_currentRoutine != null)
            {
                StopCoroutine(_currentRoutine);
                _currentRoutine = null;
            }

            _isShowing = false;
            _isHiding = false;
        }
    }
}
