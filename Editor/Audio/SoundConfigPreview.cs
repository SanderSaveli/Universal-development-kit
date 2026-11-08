using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace SanderSaveli.UDK.Audio.Editor
{
    [InitializeOnLoad]
    internal static class SoundConfigPreview
    {
        internal static bool IsActive => _config != null;
        internal static SoundConfig Config => _config;
        internal static string Description => _description;

        private const double LOAD_TIMEOUT = 10d;

        private static SoundConfig _config;
        private static SoundConfig _previewOwner;
        private static SoundConfig _previewConfig;
        private static AudioClip _clip;
        private static AudioSource _source;
        private static GameObject _previewObject;
        private static Scene _scene;
        private static PlayableGraph _graph;
        private static AudioPlayableOutput _output;
        private static bool _loop;
        private static bool _loading;
        private static bool _stopping;
        private static float _pitch;
        private static float _gain;
        private static float _fadeIn;
        private static float _fadeOut;
        private static float _stopVolume;
        private static double _duration;
        private static double _startTime;
        private static double _stopTime;
        private static string _description;

        static SoundConfigPreview()
        {
            AssemblyReloadEvents.beforeAssemblyReload += StopImmediate;
            EditorApplication.quitting += StopImmediate;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            Selection.selectionChanged += StopImmediate;
        }

        internal static void Play(SoundConfig config)
        {
            if (_previewOwner != config)
                StopImmediate();
            else
                StopPlayback();
            if (config == null || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (!EditorAudioPreviewPump.TryInitialize(out string error))
            {
                Debug.LogError(error, config);
                return;
            }

            SoundPlaybackSettings settings;
            try
            {
                if (_previewConfig == null)
                {
                    _previewConfig = UnityEngine.Object.Instantiate(config);
                    _previewConfig.hideFlags = HideFlags.HideAndDontSave;
                    _previewOwner = config;
                }
                else
                {
                    EditorUtility.CopySerialized(config, _previewConfig);
                }

                _previewConfig.name = config.name;
                settings = _previewConfig.CreatePlaybackSettings();
            }
            catch (Exception exception)
            {
                StopImmediate();
                Debug.LogException(exception, config);
                return;
            }

            if (settings.Clip.samples <= 0 || settings.Clip.frequency <= 0)
            {
                Debug.LogError("SoundConfig '" + config.name + "' selected an empty audio clip.", config);
                return;
            }

            _config = config;
            _clip = settings.Clip;
            _loop = settings.Loop;
            _pitch = settings.Pitch;
            _gain = settings.Volume;
            _fadeIn = settings.FadeIn;
            _fadeOut = settings.FadeOut;
            _duration = (double)_clip.samples / _clip.frequency / _pitch;
            if (!_loop && _fadeIn + _fadeOut > _duration)
            {
                float scale = (float)(_duration / (_fadeIn + _fadeOut));
                _fadeIn *= scale;
                _fadeOut *= scale;
            }

            _description = _clip.name + " | Volume: " + _gain.ToString("0.###")
                + " | Pitch: " + _pitch.ToString("0.###");
            _startTime = EditorApplication.timeSinceStartup;
            _loading = true;
            EditorApplication.update += Update;
            try
            {
                if (_clip.loadState == AudioDataLoadState.Unloaded && !_clip.LoadAudioData())
                    throw new InvalidOperationException("Could not load audio data.");

                if (_clip.loadState == AudioDataLoadState.Loaded)
                    StartPlayback();
            }
            catch (Exception exception)
            {
                Fail(exception.Message);
            }
        }

        internal static void Stop(SoundConfig config)
        {
            if (_config != config || _stopping)
                return;

            if (_loading || _source == null || _fadeOut <= 0f)
            {
                StopPlayback();
                return;
            }

            _stopTime = EditorApplication.timeSinceStartup;
            _stopVolume = GetEnvelope(_stopTime - _startTime);
            _stopping = true;
        }

        internal static void StopIfOwned(SoundConfig config)
        {
            if (_config == config || _previewOwner == config)
                StopImmediate();
        }

        internal static void StopImmediate()
        {
            StopPlayback();
            if (_previewConfig != null)
                UnityEngine.Object.DestroyImmediate(_previewConfig);

            _previewConfig = null;
            _previewOwner = null;
        }

        private static void StopPlayback()
        {
            EditorApplication.update -= Update;
            bool hadPlayback = _source != null || _graph.IsValid();
            if (_source != null)
            {
                _source.mute = true;
                _source.Stop();
            }

            if (_graph.IsValid())
            {
                _graph.Stop();
                if (_output.IsOutputValid())
                    _output.SetTarget(null);
                _graph.Destroy();
            }

            if (_previewObject != null)
                UnityEngine.Object.DestroyImmediate(_previewObject);

            if (_scene.IsValid())
                EditorSceneManager.ClosePreviewScene(_scene);

            _scene = default;
            _graph = default;
            _output = default;
            _source = null;
            _previewObject = null;
            _config = null;
            _clip = null;
            _loading = false;
            _stopping = false;
            _description = null;
            if (hadPlayback)
                EditorAudioPreviewPump.Update();
        }

        private static void StartPlayback()
        {
            _scene = EditorSceneManager.NewPreviewScene();
            _previewObject = EditorUtility.CreateGameObjectWithHideFlags("SoundConfig Preview",
                HideFlags.HideAndDontSave, typeof(AudioSource));
            SceneManager.MoveGameObjectToScene(_previewObject, _scene);
            _source = _previewObject.GetComponent<AudioSource>();
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _source.ignoreListenerPause = true;
            _source.ignoreListenerVolume = true;
            _source.volume = _fadeIn > 0f ? 0f : 1f;

            _graph = PlayableGraph.Create("SoundConfig Preview");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.DSPClock);
            int inputCount = Mathf.Max(1, Mathf.CeilToInt(_gain));
            AudioMixerPlayable mixer = AudioMixerPlayable.Create(_graph, inputCount, false);

            for (int i = 0; i < inputCount; i++)
            {
                AudioClipPlayable playable = AudioClipPlayable.Create(_graph, _clip, _loop);
                playable.SetSpeed(_pitch);
                playable.SetTime(0d);
                playable.SetDuration(_loop ? double.MaxValue : (double)_clip.samples / _clip.frequency);
                if (!_graph.Connect(playable, 0, mixer, i))
                    throw new InvalidOperationException("Could not connect the audio preview mixer input.");

                mixer.SetInputWeight(i, Mathf.Clamp01(_gain - i));
            }

            _output = AudioPlayableOutput.Create(_graph, "SoundConfig Preview", _source);
            _output.SetSourcePlayable(mixer);
            _output.SetEvaluateOnSeek(false);
            _graph.Play();
            _startTime = EditorApplication.timeSinceStartup;
            _loading = false;
            EditorAudioPreviewPump.Update();
        }

        private static void Update()
        {
            if (_config == null || _clip == null)
            {
                StopPlayback();
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            try
            {
                if (_loading)
                {
                    if (_clip.loadState == AudioDataLoadState.Failed || now - _startTime > LOAD_TIMEOUT)
                        Fail("Audio data failed to load or timed out.");
                    else if (_clip.loadState == AudioDataLoadState.Loaded)
                        StartPlayback();
                    return;
                }

                double elapsed = now - _startTime;
                if (_source == null || !_graph.IsValid() || (!_loop && elapsed >= _duration)
                    || (_stopping && now - _stopTime >= _fadeOut))
                {
                    StopPlayback();
                    return;
                }

                _source.volume = _stopping
                    ? _stopVolume * Mathf.Clamp01(1f - (float)((now - _stopTime) / _fadeOut))
                    : GetEnvelope(elapsed);
                EditorAudioPreviewPump.Update();
            }
            catch (Exception exception)
            {
                Fail(exception.Message);
            }
        }

        private static float GetEnvelope(double elapsed)
        {
            float fadeIn = _fadeIn <= 0f ? 1f : Mathf.Clamp01((float)(elapsed / _fadeIn));
            float fadeOut = _loop || _fadeOut <= 0f ? 1f
                : Mathf.Clamp01((float)((_duration - elapsed) / _fadeOut));
            return Mathf.Min(fadeIn, fadeOut);
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            StopImmediate();
        }

        private static void Fail(string message)
        {
            SoundConfig config = _config;
            StopImmediate();
            Debug.LogError("Sound preview failed: " + message, config);
        }
    }
}
