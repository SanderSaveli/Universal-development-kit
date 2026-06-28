using System;
using UnityEngine;

namespace SanderSaveli.UDK.Managers
{
    public interface IAudioManager
    {
        public bool IsSoundOn { get; set; }
        public bool IsMusicOn { get; set; }
        public float SoundVolume { get; set; }
        public float MusicVolume { get; set; }

        public Action<bool> OnSoundStatusChange { get; set; }
        public Action<bool> OnMusicStatusChange { get; set; }
        public Action<float> OnSoundVolumeChange { get; set; }
        public Action<float> OnMusicVolumeChange { get; set; }

        public void PlaySound(AudioClip clip);
        public void PlaySound(AudioClip clip, float volume);
        public void ChangeMusic(AudioClip clip);
    }
}
