using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SanderSaveli.UDK
{
    [Serializable]
    public struct Range
    {
        public float Min
        {
            get => Mathf.Min(_min, _max);
            set
            {
                _max = Max;
                _min = Mathf.Min(value, _max);
            }
        }
        public float Max
        {
            get => Mathf.Max(_min, _max);
            set
            {
                _min = Min;
                _max = Mathf.Max(value, _min);
            }
        }

        [SerializeField] private float _min;

        [SerializeField] private float _max;

        public Range(float min, float max)
        {
            _min = Mathf.Min(min, max);
            _max = Mathf.Max(min, max);
        }


        public float GetRandom() => UnityEngine.Random.Range(_min, _max);
    }
}
