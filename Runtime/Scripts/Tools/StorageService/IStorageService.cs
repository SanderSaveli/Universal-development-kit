using System;

namespace SanderSaveli.UDK
{
    public interface IStorageService
    {
        public bool HasFile(string key);
        public void Save(string key, object data, Action<bool> callback = null);
        public void Load<T>(string key, Action<T> callback);
        public void Delete(string key);
    }
}
