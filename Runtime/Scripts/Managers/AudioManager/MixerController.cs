using UnityEngine;
using UnityEngine.Audio;

namespace SanderSaveli.UDK
{
    public class MixerController
    {
        private const string MUSIC_VOLUME = "MusicVolume";
        private const string SOUNDS_VOLUME = "SoundsVolume";
        private const string MASTER_VOLUME = "MasterVolume";

        private const float MinVolume = 0.0001f;
        private AudioMixer _mixer;

        public MixerController(AudioMixer audioMixer)
        {
            _mixer = audioMixer;
        }

        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp(volume, MinVolume, 1f);
            _mixer.SetFloat(MASTER_VOLUME, Mathf.Log10(volume) * 20);
        }

        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp(volume, MinVolume, 1f);
            _mixer.SetFloat(MUSIC_VOLUME, Mathf.Log10(volume) * 20);
        }

        public void SetSoundVolume(float volume)
        {
            volume = Mathf.Clamp(volume, MinVolume, 1f);
            _mixer.SetFloat(SOUNDS_VOLUME, Mathf.Log10(volume) * 20);
        }

    }
}
