using System;
using UnityEngine;

namespace SanderSaveli.UDK
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MinMaxSliderAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public float Step { get; }

        public MinMaxSliderAttribute(float min, float max, float step = 0f)
        {
            Min = Mathf.Min(min, max);
            Max = Mathf.Max(min, max);
            Step = Mathf.Max(0f, step);
        }
    }
}
