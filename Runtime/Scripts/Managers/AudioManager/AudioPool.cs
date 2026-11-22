using SanderSaveli.UDK.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SanderSaveli.UDK
{
    public class AudioPool
    {
        public bool HasFreeSlot => _freeSlots.Count > 0;

        private const double COMPLETION_RETRY_SECONDS = 0.02;

        private readonly Transform _poolParent;
        private readonly AudioSlot[] _slots;
        private readonly Stack<int> _freeSlots;
        private readonly List<int> _pendingCompletions;
        private readonly List<int> _fadingSlots;
        private bool _isMuted;

        public AudioPool(Transform poolParent, int maxSimultaneousSounds)
        {
            _poolParent = poolParent;
            int capacity = Mathf.Max(1, maxSimultaneousSounds);
            _slots = new AudioSlot[capacity];
            _freeSlots = new Stack<int>(capacity);
            _pendingCompletions = new List<int>(capacity);
            _fadingSlots = new List<int>(capacity);

            for (int i = capacity - 1; i >= 0; i--)
            {
                _freeSlots.Push(i);
            }
        }

        public AudioHandle Play(SoundPlaybackSettings settings, AudioMixerGroup outputGroup)
        {
            if (settings.Clip == null || _freeSlots.Count == 0)
            {
                return default;
            }

            int slotIndex = _freeSlots.Pop();
            ref AudioSlot slot = ref _slots[slotIndex];

            if (slot.Source == null)
            {
                slot.Source = CreateNewSource();
            }

            slot.Generation++;
            slot.IsActive = true;

            AudioSource source = slot.Source;
            source.outputAudioMixerGroup = outputGroup;
            source.clip = settings.Clip;
            source.volume = SoundGain.GetSourceVolume(settings.Volume, settings.GainDb);
            slot.BaseVolume = source.volume;
            slot.FadeIn = Mathf.Max(0f, settings.FadeIn);
            slot.FadeOut = Mathf.Max(0f, settings.FadeOut);
            slot.IsStopping = false;
            slot.NaturalEndTime = double.PositiveInfinity;
            source.mute = _isMuted;
            source.pitch = settings.Pitch;
            source.loop = settings.Loop;
            double startTime = AudioSettings.dspTime;
            slot.StartTime = startTime;

            if (!source.loop)
            {
                double duration = source.clip.samples / (double)source.clip.frequency;
                float speed = Mathf.Abs(source.pitch);
                double playbackDuration = speed > 0f ? duration / speed : double.PositiveInfinity;
                double totalFade = slot.FadeIn + (double)slot.FadeOut;

                if (totalFade > playbackDuration)
                {
                    float scale = (float)(playbackDuration / totalFade);
                    slot.FadeIn *= scale;
                    slot.FadeOut *= scale;
                }

                slot.NaturalEndTime = startTime + playbackDuration;
                double endTime = startTime + (speed > 0f ? duration / speed : COMPLETION_RETRY_SECONDS);
                ScheduleCompletionCheck(slotIndex, endTime);
            }

            if (slot.FadeIn > 0f || slot.FadeOut > 0f)
            {
                _fadingSlots.Add(slotIndex);
            }

            source.volume = slot.BaseVolume * GetEnvelope(slot, startTime);
            source.Play();
            return new AudioHandle(this, slotIndex, slot.Generation);
        }

        public void ReturnCompletedSounds()
        {
            if (AudioListener.pause)
            {
                return;
            }

            double now = AudioSettings.dspTime;

            for (int i = _fadingSlots.Count - 1; i >= 0; i--)
            {
                ref AudioSlot slot = ref _slots[_fadingSlots[i]];
                slot.Source.volume = slot.BaseVolume * GetEnvelope(slot, now);
            }

            while (_pendingCompletions.Count > 0)
            {
                int slotIndex = _pendingCompletions[0];
                ref AudioSlot slot = ref _slots[slotIndex];

                if (slot.EndDspTime > now)
                {
                    return;
                }

                _pendingCompletions.RemoveAt(0);

                if (slot.IsStopping || slot.Source == null || !slot.Source.isPlaying)
                {
                    ReleaseSlot(slotIndex);
                    continue;
                }

                ScheduleCompletionCheck(slotIndex, now + COMPLETION_RETRY_SECONDS);
            }
        }

        public void Mute(bool mute)
        {
            _isMuted = mute;

            for (int i = 0; i < _slots.Length; i++)
            {
                AudioSource source = _slots[i].Source;

                if (source != null)
                {
                    source.mute = mute;
                }
            }
        }

        public void StopAll()
        {
            _pendingCompletions.Clear();

            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].IsActive)
                {
                    ReleaseSlot(i);
                }
            }
        }

        public void Clear()
        {
            StopAll();

            for (int i = 0; i < _slots.Length; i++)
            {
                AudioSource source = _slots[i].Source;

                if (source != null)
                {
                    Object.Destroy(source.gameObject);
                }

                _slots[i].Source = null;
            }
        }

        internal bool IsActive(int slotIndex, ulong generation)
        {
            ref AudioSlot slot = ref _slots[slotIndex];
            return slot.IsActive && slot.Generation == generation;
        }

        internal void StopSound(int slotIndex, ulong generation)
        {
            if (!IsActive(slotIndex, generation))
            {
                return;
            }

            ref AudioSlot slot = ref _slots[slotIndex];

            if (slot.IsStopping)
            {
                return;
            }

            if (slot.FadeOut <= 0f)
            {
                StopSoundImmediate(slotIndex, generation);
                return;
            }

            double now = AudioSettings.dspTime;
            slot.StopDuration = System.Math.Min(slot.FadeOut, System.Math.Max(0d, slot.NaturalEndTime - now));

            if (slot.StopDuration <= 0d)
            {
                StopSoundImmediate(slotIndex, generation);
                return;
            }

            slot.StopLevel = GetEnvelope(slot, now);
            slot.StopTime = now;
            slot.IsStopping = true;
            slot.Source.volume = slot.BaseVolume * slot.StopLevel;
            _pendingCompletions.Remove(slotIndex);
            ScheduleCompletionCheck(slotIndex, now + slot.StopDuration);
        }

        internal void StopSoundImmediate(int slotIndex, ulong generation)
        {
            if (!IsActive(slotIndex, generation))
            {
                return;
            }

            _pendingCompletions.Remove(slotIndex);
            ReleaseSlot(slotIndex);
        }

        private static float GetEnvelope(AudioSlot slot, double now)
        {
            if (slot.IsStopping)
            {
                return slot.StopLevel * Mathf.Clamp01(1f - (float)((now - slot.StopTime) / slot.StopDuration));
            }

            float fadeIn = slot.FadeIn > 0f
                ? Mathf.Clamp01((float)((now - slot.StartTime) / slot.FadeIn)) : 1f;
            float fadeOut = slot.FadeOut > 0f
                ? Mathf.Clamp01((float)((slot.NaturalEndTime - now) / slot.FadeOut)) : 1f;
            return Mathf.Min(fadeIn, fadeOut);
        }

        private void ScheduleCompletionCheck(int slotIndex, double endDspTime)
        {
            _slots[slotIndex].EndDspTime = endDspTime;
            int index = _pendingCompletions.Count;

            while (index > 0 && _slots[_pendingCompletions[index - 1]].EndDspTime > endDspTime)
            {
                index--;
            }

            _pendingCompletions.Insert(index, slotIndex);
        }

        private void ReleaseSlot(int slotIndex)
        {
            ref AudioSlot slot = ref _slots[slotIndex];
            slot.IsActive = false;
            _fadingSlots.Remove(slotIndex);

            if (slot.Source != null)
            {
                slot.Source.Stop();
                slot.Source.clip = null;
            }

            _freeSlots.Push(slotIndex);
        }

        private AudioSource CreateNewSource()
        {
            GameObject audioObject = new("[Poolable Audio Source]");
            audioObject.transform.SetParent(_poolParent, false);

            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private struct AudioSlot
        {
            public AudioSource Source;
            public double EndDspTime;
            public ulong Generation;
            public bool IsActive;
            public float BaseVolume;
            public double StartTime;
            public double NaturalEndTime;
            public float FadeIn;
            public float FadeOut;
            public bool IsStopping;
            public double StopTime;
            public double StopDuration;
            public float StopLevel;
        }
    }
}
