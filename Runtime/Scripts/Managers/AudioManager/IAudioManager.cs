using System;
using UnityEngine;
using SanderSaveli.UDK.Audio;

namespace SanderSaveli.UDK.Managers
{
    public interface IAudioManager
    {
        public bool IsSoundOn { get; set; }
        public bool IsMusicOn { get; set; }
        public float SoundVolume { get; set; }
        public float MusicVolume { get; set; }
        public AudioCatalog CurrentCatalog { get; }

        public Action<bool> OnSoundStatusChange { get; set; }
        public Action<bool> OnMusicStatusChange { get; set; }
        public Action<float> OnSoundVolumeChange { get; set; }
        public Action<float> OnMusicVolumeChange { get; set; }

        public AudioHandle PlaySound(AudioClip clip, float volume = 1, bool loop = false, float pitch = 1, float gainDb = 0f);
        public AudioHandle PlaySound(string id);
        public AudioHandle PlaySound(SoundId id);
        public void SetCatalog(AudioCatalog catalog);
        public void ResetCatalog();
        public void ChangeMusic(AudioClip clip);
    }
}
