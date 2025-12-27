using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public readonly struct TimerHandle
    {
        public readonly int Id;
        public TimerHandle(int id) { Id = id; }

        public void Cancel() => Timer.CancleTimer(this);
        public void Pause() => Timer.PauseTimer(this);
        public void Continue() => Timer.ContinueTimer(this);
    }
}
