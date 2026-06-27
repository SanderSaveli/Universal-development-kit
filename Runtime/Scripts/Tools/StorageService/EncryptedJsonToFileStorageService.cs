using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class EncryptedJsonToFileStorageService : IStorageService
    {
        private readonly string SECRET_KEY;
        private readonly string SALT;
        private readonly string HMAC_KEY;

        private readonly SynchronizationContext _unityContext;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();

        public EncryptedJsonToFileStorageService(string secret_key, string salt, string hmac_key)
        {
            _unityContext = SynchronizationContext.Current;
            SECRET_KEY = secret_key;
            SALT = salt;
            HMAC_KEY = hmac_key;
        }

        public void Save(string key, object data, Action<bool> callback = null)
        {
            string path = BuildPath(key);

            _ = SaveAsync(path, data, callback);
        }

        public void Load<T>(string key, Action<T> callback)
        {
            string path = BuildPath(key);

            _ = LoadAsync(path, callback);
        }

        public async Task<bool> SaveAsync(string key, object data)
        {
            string path = BuildPath(key);
            return await SaveInternalAsync(path, data);
        }

        public async Task<T> LoadAsync<T>(string key)
        {
            string path = BuildPath(key);
            return await LoadInternalAsync<T>(path);
        }

        private async Task SaveAsync(string path, object data, Action<bool> callback)
        {
            bool result = await SaveInternalAsync(path, data);
            InvokeOnMainThread(() => callback?.Invoke(result));
        }

        private async Task LoadAsync<T>(string path, Action<T> callback)
        {
            T result = await LoadInternalAsync<T>(path);
            InvokeOnMainThread(() => callback?.Invoke(result));
        }

        private async Task<bool> SaveInternalAsync(string path, object data)
        {
            SemaphoreSlim semaphore = _fileLocks.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();

            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));

                        string json = JsonConvert.SerializeObject(data);
                        byte[] encrypted = Encrypt(json);
                        byte[] hmac = ComputeHMAC(encrypted);

                        using (var fs = new FileStream(
                            path,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None))
                        {
                            fs.Write(hmac, 0, hmac.Length);
                            fs.Write(encrypted, 0, encrypted.Length);
                        }

                        Debug.Log("Save success " + path);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        return false;
                    }
                });
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<T> LoadInternalAsync<T>(string path)
        {
            SemaphoreSlim semaphore = _fileLocks.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();

            try
            {
                return await Task.Run(() =>
                {
                    try
                    {
                        if (!File.Exists(path))
                            return default;

                        byte[] fileBytes = File.ReadAllBytes(path);

                        if (fileBytes.Length < 32)
                        {
                            Debug.LogError("Save file corrupted!");
                            return default;
                        }

                        byte[] storedHmac = new byte[32];
                        byte[] encrypted = new byte[fileBytes.Length - 32];

                        Array.Copy(fileBytes, 0, storedHmac, 0, 32);
                        Array.Copy(fileBytes, 32, encrypted, 0, encrypted.Length);

                        byte[] computedHmac = ComputeHMAC(encrypted);

                        if (!CompareBytes(storedHmac, computedHmac))
                        {
                            Debug.LogError("Save file tampered!");
                            return default;
                        }

                        string json = Decrypt(encrypted);
                        return JsonConvert.DeserializeObject<T>(json);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                        return default;
                    }
                });
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void InvokeOnMainThread(Action action)
        {
            if (_unityContext != null)
                _unityContext.Post(_ => action?.Invoke(), null);
            else
                action?.Invoke();
        }

        private byte[] Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(
                    SECRET_KEY,
                    Encoding.UTF8.GetBytes(SALT),
                    10000
                );

                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return ms.ToArray();
                }
            }
        }

        private string Decrypt(byte[] cipherData)
        {
            using (Aes aes = Aes.Create())
            {
                var key = new Rfc2898DeriveBytes(
                    SECRET_KEY,
                    Encoding.UTF8.GetBytes(SALT),
                    10000
                );

                aes.Key = key.GetBytes(32);
                aes.IV = key.GetBytes(16);

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherData))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private byte[] ComputeHMAC(byte[] data)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(HMAC_KEY)))
            {
                return hmac.ComputeHash(data);
            }
        }

        private bool CompareBytes(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int result = 0;

            for (int i = 0; i < a.Length; i++)
                result |= a[i] ^ b[i];

            return result == 0;
        }

        private string BuildPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key + ".dat");
        }
    }
}