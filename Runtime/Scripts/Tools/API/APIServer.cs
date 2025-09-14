using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SanderSaveli.UDK
{
    public static class APIServer
    {
        private static IEnumerator SendRequest<T>(
            string url,
            string method,
            string body,
            Action<T> callback = null,
            Action<string> error = null)
        {
            UnityWebRequest request;

            if (method == UnityWebRequest.kHttpVerbGET)
            {
                request = UnityWebRequest.Get(url);
            }
            else
            {
                request = new UnityWebRequest(url, method);
                if (!string.IsNullOrEmpty(body))
                {
                    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(body);
                    request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                }
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
            }
            UnityEngine.Debug.Log($"URL : {url} \n Method : {method} \n Body : {body}");
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                UnityEngine.Debug.LogError($"Responce error witn status {request.result} : {request.error}");
                error?.Invoke(request.error);
            }
            else
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        T response = JsonConvert.DeserializeObject<T>(responseText);
                        UnityEngine.Debug.Log($"Responce for request {url} : \n {responseText}");
                        callback?.Invoke(response);
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"Responce for request {url} without downloadHandler");
                        callback?.Invoke(default);
                    }
                }
                catch (Exception ex)
                {
                    error?.Invoke($"Deserialization error: {ex.Message}");
                }
            }

            request.Dispose();
        }

        public static IEnumerator GET<T>(string url, Action<T> callback = null, Action<string> error = null)
        {
            yield return SendRequest(url, UnityWebRequest.kHttpVerbGET, null, callback, error);
        }

        public static IEnumerator POST<T>(string url, string body, Action<T> callback = null, Action<string> error = null)
        {
            yield return SendRequest(url, UnityWebRequest.kHttpVerbPOST, body, callback, error);
        }

        public static IEnumerator PATCH<T>(string url, string body, Action<T> callback = null, Action<string> error = null)
        {
            yield return SendRequest(url, "PATCH", body, callback, error);
        }

        public static IEnumerator DELETE<T>(string url, string body, Action<T> callback = null, Action<string> error = null)
        {
            yield return SendRequest(url, UnityWebRequest.kHttpVerbDELETE, body, callback, error);
        }
    }
}
