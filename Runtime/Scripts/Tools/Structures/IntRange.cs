using System;
using UnityEngine;

namespace SanderSaveli.UDK
{
    [Serializable]
    public struct IntRange
    {
        public int Min
        {
            get => _min;
            set
            {
                _min = Mathf.Min(value, _max);
            }
        }
        public int Max
        {
            get => _max;
            set
            {
                _max = Mathf.Max(value, _min);
            }
        }

        [SerializeField] private int _min;
        [SerializeField] private int _max;

        public IntRange(int min, int max)
        {
            _min = Mathf.Min(min, max);
            _max = Mathf.Max(min, max);
        }

        public int GetRandom() => UnityEngine.Random.Range(_min, _max);
    }
}
