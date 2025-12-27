using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.GravityMaze
{
    public class StepSlider : Slider
    {
        public float Step = 0.05f;

        protected override void Set(float input, bool sendCallback)
        {
            float steppedValue = GetSteppedValue(input);
            base.Set(steppedValue, sendCallback);
        }

        private float GetSteppedValue(float value)
        {
            if (Step <= 0f)
                return value;

            float min = minValue;
            float max = maxValue;

            float stepped =
                min + Mathf.Round((value - min) / Step) * Step;

            return Mathf.Clamp(stepped, min, max);
        }
    }
}
