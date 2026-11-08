using System;
using UnityEngine;

namespace SanderSaveli.UDK
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MinMaxSliderAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public MinMaxSliderAttribute(float min, float max)
        {
            Min = Mathf.Min(min, max);
            Max = Mathf.Max(min, max);
        }
    }
}
