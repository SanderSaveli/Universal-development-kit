using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SanderSaveli.UDK.Audio
{
    [CreateAssetMenu(menuName = "UDK/Audio/SoundConfig", fileName ="new Sound Config")]
    public class SoundConfig : ScriptableObject
    {
        public string Id => _id;
        public IReadOnlyList<ClipConfig> Clips => _clips;
        public bool AvoidRepetition => _avoidRepetition;
        public bool Loop => _loop;
        public float Cooldown => _minCooldown;
        public float FadeIn => _fadeIn;
        public float FadeOut => _fadeOut;
        public Range VolumeRange => _volumeRange;
        public Range PitchRange => _pitchRange;

        [SerializeField] private string _id;

        [Header("Clips")]
        [SerializeField] private List<ClipConfig> _clips = new List<ClipConfig>();
        [Tooltip("The same list element will not be selected consecutively.")]
        [SerializeField] private bool _avoidRepetition;

        [Header("Playing")]
        [SerializeField] private bool _loop;
        [Tooltip("Min time between playbacks.")]
        [SerializeField, Min(0f)] private float _minCooldown;
        [SerializeField, Min(0f)] private float _fadeIn;
        [SerializeField, Min(0f)] private float _fadeOut;

        [Header("Random")]
        [SerializeField, MinMaxSlider(0f, 2f)] 
        private Range _volumeRange = new Range(1f, 1f);
        [SerializeField, MinMaxSlider(0.01f, 2f)]
        private Range _pitchRange = new Range(1f, 1f);

        [NonSerialized] private int _previousClipIndex = -1;

        public SoundPlaybackSettings CreatePlaybackSettings()
        {
            int index = ChooseClipIndex();
            ClipConfig clip = _clips[index];

            if(clip == null)
            {
                throw new InvalidOperationException($"SoundConfig \"{name}\" ID: {_id} has null clip in index {index}.");
            }

            float volume = clip.Volume * _volumeRange.GetRandom();
            float pitch = _pitchRange.GetRandom();
            _previousClipIndex = index;

            return new SoundPlaybackSettings(clip.Clip, volume, pitch, _loop,
                _fadeIn, _fadeOut);
        }

        private int ChooseClipIndex()
        {
            int index;

            do
            {
                index = Random.Range(0, _clips.Count);
            } while (_clips.Count > 1 && _avoidRepetition && index == _previousClipIndex);
            
            return index;
        }
    }
}
