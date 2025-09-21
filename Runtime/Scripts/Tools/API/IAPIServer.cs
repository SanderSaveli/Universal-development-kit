using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SanderSaveli.UDK
{
    public interface IAPIServer
    {
        public static bool EnableLogging { get; set; }

        public IEnumerator GET(
            string url,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);

        public IEnumerator POST(
            string url,
            string body,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);

        public IEnumerator PATCH(
            string url,
            string body,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);

        public IEnumerator DELETE(
            string url,
            string body = null,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);

        public IEnumerator RequestRaw(
            string url,
            string method,
            string body = null,
            Action<string> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);

        public IEnumerator GetTexture(
            string url,
            Action<Texture2D> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);

        public IEnumerator RequestBytes(
            string url,
            string method = UnityWebRequest.kHttpVerbGET,
            string body = null,
            Action<byte[]> callback = null,
            Action<string> error = null,
            Dictionary<string, string> headers = null);
    }
}
