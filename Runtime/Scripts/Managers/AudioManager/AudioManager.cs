using System;
using UnityEngine;

namespace SanderSaveli.UDK.Managers
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        public bool IsSoundOn { get => _isSoundOn; set => ChangeSoundStatus(value); }
        public bool IsMusicOn { get => _isMusicOn; set => ChangeMusicStatus(value); }
        public float SoundVolume { get => _soundVolume; set => ChangeSoundVolume(value); }
        public float MusicVolume { get => _musicVolume; set => ChangeMusicVolume(value); }

        public Action<bool> OnSoundStatusChange { get; set; }
        public Action<bool> OnMusicStatusChange { get; set; }
        public Action<float> OnSoundVolumeChange { get; set; }
        public Action<float> OnMusicVolumeChange { get; set; }

        [SerializeField] private AudioSource _soundAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;

        private bool _isSoundOn;
        private bool _isMusicOn;
        private float _soundVolume;
        private float _musicVolume;

        public void PlaySound(AudioClip clip)
        {
            _soundAudioSource.PlayOneShot(clip);
        }

        public void PlaySound(AudioClip clip, float volume)
        {
            _soundAudioSource.PlayOneShot(clip, volume);
        }

        public void ChangeMusic(AudioClip clip)
        {
            _musicAudioSource.clip = clip;
        }

        private void ChangeSoundStatus(bool isActive)
        {
            if (IsSoundOn == isActive)
            {
                return;
            }
            _isSoundOn = isActive;
            _soundAudioSource.mute = !_isSoundOn;
            OnSoundStatusChange?.Invoke(_isSoundOn);
        }

        private void ChangeMusicStatus(bool isActive)
        {
            if (IsMusicOn == isActive)
            {
                return;
            }
            _isMusicOn = isActive;
            _musicAudioSource.mute = !_isMusicOn;
            OnMusicStatusChange?.Invoke(_isMusicOn);
        }

        private void ChangeSoundVolume(float volume)
        {
            if (SoundVolume == volume)
            {
                return;
            }
            _soundVolume = volume;
            _soundAudioSource.volume = _soundVolume;
            OnSoundVolumeChange?.Invoke(_soundVolume);
        }

        private void ChangeMusicVolume(float volume)
        {
            if (MusicVolume == volume)
            {
                return;
            }
            _musicVolume = volume;
            _musicAudioSource.volume = _musicVolume;
            OnMusicVolumeChange?.Invoke(_musicVolume);
        }
    }
}
