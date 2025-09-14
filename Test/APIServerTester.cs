using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json; // убедись, что пакет подключен
using SanderSaveli.UDK;

public class APIServerTester : MonoBehaviour
{
    private void Start()
    {
        // Запускаем тесты
        StartCoroutine(StartTests());
    }

    [Serializable]
    public class TestResponse
    {
        public int userId;
        public int id;
        public string title;
        public string body;
    }

    private IEnumerator StartTests()
    {
        yield return StartCoroutine(TestGET());
        yield return StartCoroutine(TestPOST());
        yield return StartCoroutine(TestPATCH());
        yield return StartCoroutine(TestDELETE());
        yield return StartCoroutine(TestError());
    }

    private IEnumerator TestGET()
    {
        string url = "https://jsonplaceholder.typicode.com/posts/1";
        yield return APIServer.GET<TestResponse>(
            url,
            (TestResponse) => Debug.Log($"✅ GET success: title"),
            (error) => Debug.LogError($"❌ GET error: {error}")
        );
    }

    private IEnumerator TestPOST()
    {
        string url = "https://jsonplaceholder.typicode.com/posts";
        var body = JsonConvert.SerializeObject(new { title = "foo", body = "bar", userId = 1 });

        yield return APIServer.POST<TestResponse>(
            url,
            body,
            onSuccess => Debug.Log($"✅ POST success: created id = {onSuccess.id}"),
            error => Debug.LogError($"❌ POST error: {error}")
        );
    }

    private IEnumerator TestPATCH()
    {
        string url = "https://jsonplaceholder.typicode.com/posts/1";
        var body = JsonConvert.SerializeObject(new { title = "updated title" });

        yield return APIServer.PATCH<TestResponse>(
            url,
            body,
            onSuccess => Debug.Log($"✅ PATCH success: new title = {onSuccess.title}"),
            error => Debug.LogError($"❌ PATCH error: {error}")
        );
    }

    private IEnumerator TestDELETE()
    {
        string url = "https://jsonplaceholder.typicode.com/posts/1";

        yield return APIServer.DELETE<string>(
            url,
            null,
            onSuccess => Debug.Log($"✅ DELETE success, response: {onSuccess}"),
            error => Debug.LogError($"❌ DELETE error: {error}")
        );
    }

    private IEnumerator TestError()
    {
        string url = "https://jsonplaceholder.typicode.com/invalid_endpoint";

        yield return APIServer.GET<TestResponse>(
            url,
            onSuccess => Debug.Log($"❌ ERROR TEST should not succeed, got: {onSuccess}"),
            error => Debug.LogWarning($"✅ Expected error handled: {error}")
        );
    }
}
