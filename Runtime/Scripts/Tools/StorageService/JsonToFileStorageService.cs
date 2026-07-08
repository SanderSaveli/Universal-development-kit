using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class JsonToFileStorageService : IStorageService
    {
        public bool HasFile(string key)
        {
            string path = BuildPath(key);
            return File.Exists(path);
        }

        public void Save(string key, object data, Action<bool> callback = null)
        {
            string path = BuildPath(key);
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string jsonFile = JsonConvert.SerializeObject(data);

            using (var fileStream = new StreamWriter(path))
            {
                fileStream.Write(jsonFile);
            }
            callback?.Invoke(true);
        }

        public void Load<T>(string key, Action<T> callback)
        {
            string path = BuildPath(key);

            using (var fileStream = new StreamReader(path))
            {
                string jsonFile = fileStream.ReadToEnd();
                T data = JsonConvert.DeserializeObject<T>(jsonFile);

                callback.Invoke(data);
            }
        }

        public void Delete(string key)
        {
            string path = BuildPath(key);

            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        private string BuildPath(string key) =>
             Path.Combine(Application.persistentDataPath, key);
    }
}
