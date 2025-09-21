using System;

namespace SanderSaveli.UDK
{
    public interface IStorageService
    {
        public void Save(string key, object data, Action<bool> callback = null);
        public void Load<T>(string key, Action<T> callback);
    }
}
