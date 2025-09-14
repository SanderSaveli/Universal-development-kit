using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public readonly struct TimerHandle
    {
        public readonly int Id;
        public TimerHandle(int id) { Id = id; }

        public void Cancel() => Timer.Cancel(this);
    }
}
