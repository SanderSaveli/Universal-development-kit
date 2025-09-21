using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class JsonToStreamingAssetsStorageService : MonoBehaviour, IStorageService
    {
        public void Save(string key, object data, Action<bool> callback = null)
        {
            string tempPath = BuildTempPath(key);
            string jsonFile = JsonConvert.SerializeObject(data, Formatting.Indented);

            try
            {
                string tempDirectory = Path.GetDirectoryName(tempPath);
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }

                using (var fileStream = new StreamWriter(tempPath))
                {
                    fileStream.Write(jsonFile);
                }

                string streamingAssetsPath = BuildStreamingAssetsPath(key);

                string streamingAssetsDirectory = Path.GetDirectoryName(streamingAssetsPath);
                if (!Directory.Exists(streamingAssetsDirectory))
                {
                    Directory.CreateDirectory(streamingAssetsDirectory);
                }

                File.Copy(tempPath, streamingAssetsPath, true);
                File.Delete(tempPath);

                callback?.Invoke(true);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in data save: {ex.Message}");
                callback?.Invoke(false);
            }
        }

        public void Load<T>(string key, Action<T> callback)
        {
            string path = BuildStreamingAssetsPath(key);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"There is no file {key} in " + BuildStreamingAssetsPath(key));
                callback.Invoke(default(T));
                return;
            }

            try
            {
                using (var fileStream = new StreamReader(path))
                {
                    string jsonFile = fileStream.ReadToEnd();
                    T data = JsonConvert.DeserializeObject<T>(jsonFile);
                    callback?.Invoke(data);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error with load from StreamingAssets: {ex.Message}");
                callback?.Invoke(default(T));
            }
        }

        private string BuildTempPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key + ".json");
        }

        private string BuildStreamingAssetsPath(string key)
        {
            return Path.Combine(Application.streamingAssetsPath, key + ".json");
        }
    }
}
