using System;

namespace SanderSaveli.UDK
{
    public interface IPoolableObject<T>
    {
        public Action<T> OnBackToPool { get; set; }
        public void OnActive();
    }
}
