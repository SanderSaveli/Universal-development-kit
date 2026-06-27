using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SanderSaveli.UDK
{
    public class JsonToResourcesStorageService : IStorageService
    {
        private const string ResourcesFolder = "Assets/Resources/";

        public void Save(string key, object data, Action<bool> callback = null)
        {
#if UNITY_EDITOR
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);

                string fullPath = Path.Combine(ResourcesFolder, key + ".json");
                string directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(fullPath, json);

                AssetDatabase.Refresh();

                Debug.Log($"Saved to Resources: {fullPath}");
                callback?.Invoke(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save error: {e.Message}");
                callback?.Invoke(false);
            }
#else
            Debug.LogWarning("Save is not supported in build for Resources");
            callback?.Invoke(false);
#endif
        }

        public void Load<T>(string key, Action<T> callback)
        {
            try
            {
                TextAsset asset = Resources.Load<TextAsset>(key);

                if (asset == null)
                {
                    Debug.LogWarning($"File not found in Resources: {key}");
                    callback?.Invoke(default);
                    return;
                }

                T data = JsonConvert.DeserializeObject<T>(asset.text);
                callback?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Load error: {e.Message}");
                callback?.Invoke(default);
            }
        }
    }
}