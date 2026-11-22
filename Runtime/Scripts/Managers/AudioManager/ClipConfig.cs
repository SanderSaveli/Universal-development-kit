using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SanderSaveli.UDK.Audio
{
    [Serializable]
    public class ClipConfig
    {
        public AudioClip Clip => _clip;
        public float GainDb => _gainDb;

        [FormerlySerializedAs("Clip"), SerializeField] private AudioClip _clip;

        [SerializeField, Range(SoundGain.MIN_GAIN_DB, SoundGain.MAX_GAIN_DB)] private float _gainDb;
    }
}
