namespace SceneLoad
{
    public interface ITransitionAnimator
    {
        public void PlayTransistAnimation(int transitionIndex, out float animationDuration);

        public void PlayTransistAnimationReverse(int transitionIndex, out float animationDuration);
    }
}

