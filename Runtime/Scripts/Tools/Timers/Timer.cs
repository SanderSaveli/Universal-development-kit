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

        private class TimerItem
        {
            public int Id;
            public float Remaining;
            public bool IsRealtime;
            public bool IsOnPause;
            public Action OnComplete;
            public Action<float> OnTick;
        }

        private static Dictionary<int, TimerItem> _timers = new Dictionary<int, TimerItem>();
        private static int _nextId = 1;
        private static Runner _runner;
        private static GameObject _runnerGO;
        private static List<TimerItem> _completedTimers = new List<TimerItem>();

        public static TimerHandle StartTimer(float seconds, Action onComplete, Action<float> onTick = null)
        {
            return Start(seconds, onComplete, onTick, false);
        }

        public static TimerHandle StartTimerRealtime(float seconds, Action onComplete, Action<float> onTick = null)
        {
            return Start(seconds, onComplete, onTick, true);
        }

        public static bool CancleTimer(TimerHandle handle)
        {
            return RemoveById(handle.Id);
        }

        public static void PauseTimer(TimerHandle handle)
        {
            if (!_timers.TryGetValue(handle.Id, out TimerItem item))
                return;
            item.IsOnPause = true;
        }

        public static void ContinueTimer(TimerHandle handle)
        {
            if (!_timers.TryGetValue(handle.Id, out TimerItem item))
                return;
            item.IsOnPause = false;
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
                IsOnPause = false,
            };

            _timers.Add(id, item);

            return new TimerHandle(id);
        }

        private static void EnsureRunner()
        {
            if (_runner != null) return;

            _runnerGO = new GameObject("[Timer_Runner]");
            _runnerGO.hideFlags = HideFlags.HideAndDontSave;
            _runner = _runnerGO.AddComponent<Runner>();
            GameObject.DontDestroyOnLoad(_runnerGO);
        }

        private static bool RemoveById(int id)
        {
            if( _timers.ContainsKey(id))
            {
                _timers.Remove(id);
                return true;
            }
            return false;
        }

        private static void UpdateTimers()
        {
            if (_timers.Count == 0) return;

            foreach(TimerItem timer in _timers.Values)
            {
                if(timer.IsOnPause) continue;

                float delataTime = timer.IsRealtime ? Time.unscaledDeltaTime : Time.deltaTime;
                TickTimer(timer, delataTime);

                if (timer.Remaining <= 0f)
                {
                    CompleteTimer(timer);
                    _completedTimers.Add(timer);
                }
            }

            foreach(TimerItem timer in _completedTimers)
            {
                RemoveById(timer.Id);
            }
            _completedTimers.Clear();
        }

        private static void TickTimer(TimerItem timer, float deltaTime)
        {
            timer.Remaining -= deltaTime;
            float remainToReport = Mathf.Max(0f, timer.Remaining);
            try
            {
                timer.OnTick?.Invoke(remainToReport);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void CompleteTimer(TimerItem timer)
        {
            try
            {
                timer.OnComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
