using UnityEngine;

namespace SanderSaveli.UDK.Audio
{
    public readonly struct SoundPlaybackSettings
    {
        public AudioClip Clip { get; }
        public float Volume { get; }
        public float GainDb { get; }
        public float Pitch { get; }
        public bool Loop { get; }
        public float FadeIn { get; }
        public float FadeOut { get; }

        public SoundPlaybackSettings(AudioClip clip, float volume, float pitch, bool loop, float fadeIn, float fadeOut, float gainDb = 0f)
        {
            Clip = clip;
            Volume = Mathf.Clamp01(volume);
            GainDb = gainDb;
            Pitch = pitch;
            Loop = loop;
            FadeIn = fadeIn;
            FadeOut = fadeOut;
        }
    }
}
