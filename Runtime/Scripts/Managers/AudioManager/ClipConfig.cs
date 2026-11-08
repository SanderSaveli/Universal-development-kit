using System;
using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    [Serializable]
    public class ClipConfig
    {
        public AudioClip Clip => _clip;
        public float Volume => _volume;

        [SerializeField] private AudioClip _clip;

        [Range(0f, 2f), Min(0)]
        [SerializeField] private float _volume = 1f;
    }
}
