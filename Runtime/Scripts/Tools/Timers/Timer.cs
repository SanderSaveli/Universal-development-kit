using System;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public static class Timer
    {
        private class Runner : MonoBehaviour
        {
            private void Update() => UpdateTimers();
        }

        private struct TimerItem
        {
            public int Id;
            public float Remaining;
            public Action OnComplete;
            public Action<float> OnTick;
            public bool IsRealtime;
        }

        private static List<TimerItem> _timers = new List<TimerItem>();
        private static Dictionary<int, int> _idToIndex = new Dictionary<int, int>();
        private static int _nextId = 1;
        private static Runner _runner;
        private static GameObject _runnerGO;

        private static void EnsureRunner()
        {
            if (_runner != null) return;

            _runnerGO = new GameObject("[Timer_Runner]");
            _runnerGO.hideFlags = HideFlags.HideAndDontSave;
            _runner = _runnerGO.AddComponent<Runner>();
            GameObject.DontDestroyOnLoad(_runnerGO);
        }

        public static TimerHandle StartTimer(float seconds, Action onComplete, Action<float> onTick = null)
        {
            return Start(seconds, onComplete, onTick, false);
        }

        public static TimerHandle StartTimerRealtime(float seconds, Action onComplete, Action<float> onTick = null)
        {
            return Start(seconds, onComplete, onTick, true);
        }

        private static TimerHandle Start(float seconds, Action onComplete, Action<float> onTick = null, bool realtime = false)
        {
            if(seconds <= 0f)
            {
                throw new ArgumentException($"Timer duration must be > 0, given duration: {seconds}");
            }

            EnsureRunner();

            int id = _nextId++;
            TimerItem item = new TimerItem
            {
                Id = id,
                Remaining = seconds,
                OnComplete = onComplete,
                OnTick = onTick,
                IsRealtime = realtime,
            };

            int index = _timers.Count;
            _timers.Add(item);
            _idToIndex[id] = index;

            return new TimerHandle(id);
        }

        public static bool Cancel(TimerHandle handle)
        {
            return RemoveById(handle.Id);
        }

        private static bool RemoveById(int id)
        {
            if (!_idToIndex.TryGetValue(id, out int index))
                return false;

            int lastIndex = _timers.Count - 1;
            if (index != lastIndex)
            {
                var last = _timers[lastIndex];
                _timers[index] = last;
                _idToIndex[last.Id] = index;
            }

            _timers.RemoveAt(lastIndex);
            _idToIndex.Remove(id);
            return true;
        }

        private static void UpdateTimers()
        {
            if (_timers.Count == 0) return;

            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                TimerItem timer = _timers[i];
                timer.Remaining -= timer.IsRealtime ? Time.unscaledDeltaTime : Time.deltaTime;

                float remainToReport = Mathf.Max(0f, timer.Remaining);
                try
                {
                    timer.OnTick?.Invoke(remainToReport);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (timer.Remaining <= 0f)
                {
                    try
                    {
                        timer.OnComplete?.Invoke();
                    }
                    catch (Exception e) 
                    { 
                        Debug.LogException(e); 
                    }
                    RemoveById(timer.Id);
                }
                else
                {
                    _timers[i] = timer;
                }
            }
        }
    }
}
