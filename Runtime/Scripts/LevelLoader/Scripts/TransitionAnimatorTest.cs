using UnityEngine;

namespace SceneLoad
{
    public class TransitionAnimatorTest : MonoBehaviour
    {
        [SerializeField] private TransitionAnimator transitionAnimator;
        public int maskIndex;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                transitionAnimator.PlayTransistAnimation(maskIndex, out float animationDuration);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                transitionAnimator.PlayTransistAnimationReverse(maskIndex, out float animationDuration);
            }
        }
    }
}

