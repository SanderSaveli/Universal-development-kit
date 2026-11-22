using SanderSaveli.UDK.Audio;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SanderSaveli.UDK.Managers
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        public bool IsSoundOn { get => _isSoundOn; set => ChangeSoundStatus(value); }
        public bool IsMusicOn { get => _isMusicOn; set => ChangeMusicStatus(value); }
        public float SoundVolume { get => _soundVolume; set => ChangeSoundVolume(value); }
        public float MusicVolume { get => _musicVolume; set => ChangeMusicVolume(value); }

        public AudioCatalog CurrentCatalog => _audioCatalog;
        public AudioCatalog AudioCatalog { get => CurrentCatalog; set => SetCatalog(value); }

        public Action<bool> OnSoundStatusChange { get; set; }
        public Action<bool> OnMusicStatusChange { get; set; }
        public Action<float> OnSoundVolumeChange { get; set; }
        public Action<float> OnMusicVolumeChange { get; set; }
        public Action<AudioCatalog> OnAudioCatalogChange { get; set; }

        [Header("General")]
        [SerializeField] private AudioCatalog _defaultCatalog;
        [SerializeField] private AudioMixer _audioMixer;
        [Min(10)]
        [SerializeField] private int _maxSimultaneousSounds = 40;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicAudioSource;

        private bool _isSoundOn;
        private bool _isMusicOn;
        private float _soundVolume;
        private float _musicVolume;

        private AudioCatalog _audioCatalog;
        private MixerController _mixerController;
        private AudioPool _audioPool;
        private AudioMixerGroup _soundGroup;
        private readonly Dictionary<string, double> _lastPlayTimes = new Dictionary<string, double>(StringComparer.Ordinal);

        private void Awake()
        {
            ResetCatalog();
            _mixerController = new MixerController(_audioMixer);

            _soundGroup = FindMixerGroup(SoundGain.MIXER_GROUP_PATH);
            AudioMixerGroup musicGroup = FindMixerGroup("Master/Music");

            _musicAudioSource.outputAudioMixerGroup = musicGroup;

            _audioPool = new AudioPool(transform, _maxSimultaneousSounds);
            _audioPool.Mute(!_isSoundOn);
        }

        private void Update()
        {
            _audioPool.ReturnCompletedSounds();
        }

        private void OnDisable()
        {
            _audioPool?.StopAll();
        }

        private void OnDestroy()
        {
            _audioPool?.Clear();
        }

        public AudioHandle PlaySound(string id)
        {
            if (!isActiveAndEnabled || _audioCatalog == null || string.IsNullOrEmpty(id))
            {
                return default;
            }

            if (_audioCatalog.TryGetSound(id, out SoundConfig soundConfig))
            {
                double now = AudioSettings.dspTime;

                if (_lastPlayTimes.TryGetValue(id, out double lastPlayTime)
                    && now - lastPlayTime < soundConfig.Cooldown)
                {
                    return default;
                }

                if (!_audioPool.HasFreeSlot)
                {
                    return default;
                }

                AudioHandle handle = Play(soundConfig.CreatePlaybackSettings());

                if (handle.IsPlaying)
                {
                    _lastPlayTimes[id] = now;
                }

                return handle;
            }

            Debug.LogWarning($"Cant't find sound with id {id} in {_audioCatalog.name}");
            return default;
        }

        public AudioHandle PlaySound(SoundId id)
        {
            return PlaySound(id.Value);
        }

        public void SetCatalog(AudioCatalog catalog)
        {
            if (catalog == null || !catalog.IsValid())
            {
                Debug.LogError("Cannot assign an invalid audio catalog.", this);
                return;
            }

            if (_audioCatalog == catalog)
            {
                return;
            }

            _audioCatalog = catalog;
            OnAudioCatalogChange?.Invoke(_audioCatalog);
        }

        public void ResetCatalog()
        {
            SetCatalog(_defaultCatalog);
        }

        public AudioHandle PlaySound(AudioClip clip, float volume = 1, bool loop = false, float pitch = 1, float gainDb = 0f)
        {
            SoundPlaybackSettings soundSettings = new SoundPlaybackSettings(clip, volume, pitch, loop, 0, 0, gainDb);
            return Play(soundSettings);
        }

        public void ChangeMusic(AudioClip clip)
        {
            _musicAudioSource.clip = clip;
        }

        private AudioMixerGroup FindMixerGroup(string path)
        {
            AudioMixerGroup[] groups = _audioMixer.FindMatchingGroups(path);

            if (groups.Length != 1)
                throw new InvalidOperationException(
                    $"Expected one mixer group matching '{path}' in '{_audioMixer.name}'.");

            return groups[0];
        }

        private void ChangeSoundStatus(bool isActive)
        {
            if (IsSoundOn == isActive)
            {
                return;
            }
            _isSoundOn = isActive;
            _audioPool.Mute(!_isSoundOn);
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
            _mixerController.SetSoundVolume(volume);
            OnSoundVolumeChange?.Invoke(_soundVolume);
        }

        private void ChangeMusicVolume(float volume)
        {
            if (MusicVolume == volume)
            {
                return;
            }
            _musicVolume = volume;
            _mixerController.SetMusicVolume(volume);
            OnMusicVolumeChange?.Invoke(_musicVolume);
        }

        private AudioHandle Play(SoundPlaybackSettings soundSettings)
        {
            return isActiveAndEnabled ? _audioPool.Play(soundSettings, _soundGroup) : default;
        }
    }
}
