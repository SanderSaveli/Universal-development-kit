using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace SanderSaveli.UDK
{
    public static class APIServer
    {
        public static bool EnableLogging { get; set; } = true;

        public static IEnumerator GET(
            string url,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
            => RequestRaw(url, UnityWebRequest.kHttpVerbGET, null, callback, error, headers);

        public static IEnumerator POST(
            string url,
            string body,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
            => RequestRaw(url, UnityWebRequest.kHttpVerbPOST, body, callback, error, headers);

        public static IEnumerator PATCH(
            string url,
            string body,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
            => RequestRaw(url, "PATCH", body, callback, error, headers);

        public static IEnumerator DELETE(
            string url,
            string body = null,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
            => RequestRaw(url, UnityWebRequest.kHttpVerbDELETE, body, callback, error, headers);

        public static IEnumerator RequestRaw(
            string url,
            string method,
            string body = null,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
        {
            using (UnityWebRequest request = BuildRequest(url, method, body, headers))
            {
                if (EnableLogging)
                    Debug.Log($"[APIServer] URL: {url}\nMethod: {method}\nBody: {body}");

                yield return request.SendWebRequest();
                HandleResponse(request, callback, error);
            }
        }

        public static IEnumerator GetTexture(
            string url,
            Action<Texture2D> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                        request.SetRequestHeader(header.Key, header.Value);
                }

                yield return request.SendWebRequest();
                HandleResponse(request,
                    _ => callback?.Invoke(DownloadHandlerTexture.GetContent(request)),
                    error);
            }
        }

        public static IEnumerator RequestBytes(
            string url,
            string method = UnityWebRequest.kHttpVerbGET,
            string body = null,
            Action<byte[]> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null)
        {
            using (UnityWebRequest request = BuildRequest(url, method, body, headers))
            {
                yield return request.SendWebRequest();
                HandleResponse(request,
                    _ => callback?.Invoke(request.downloadHandler?.data),
                    error);
            }
        }

        private static UnityWebRequest BuildRequest(
            string url,
            string method,
            string body,
            Dictionary<string, string> headers)
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
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }
                request.downloadHandler = new DownloadHandlerBuffer();
            }

            AddHeaders(request, headers, !string.IsNullOrEmpty(body));
            return request;
        }

        private static void HandleResponse(
            UnityWebRequest request,
            Action<string> callback,
            Action<string> error)
        {
#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                if (EnableLogging)
                    Debug.LogError($"[APIServer] Error ({request.result}): {request.error}");

                error?.Invoke(request.error);
            }
            else
            {
                string responseText = request.downloadHandler?.text;

                if (EnableLogging)
                    Debug.Log($"[APIServer] Response from {request.url}:\n{responseText}");

                callback?.Invoke(responseText);
            }
        }

        private static void AddHeaders(UnityWebRequest request, Dictionary<string, string> headers, bool hasBody)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                    request.SetRequestHeader(header.Key, header.Value);
            }

            if (hasBody && (headers == null || !headers.ContainsKey("Content-Type")))
            {
                request.SetRequestHeader("Content-Type", "application/json");
            }
        }
    }
}
