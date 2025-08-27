using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public class URLImage : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void GetImageByURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogWarning("Image Url is empty!");
                return;
            }
            if (gameObject.activeInHierarchy)
            {
                _image.sprite = null;
                _image.color = new Color(0, 0, 0, 0);
                StartCoroutine(GetTexture(url));
            }
        }

        private IEnumerator GetTexture(string url)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
                HandleTexture(myTexture);
            }
        }

        private void HandleTexture(Texture2D tex)
        {
            Rect spriteRect = new Rect(0, 0, tex.width, tex.height);
            Vector2 pivot = _image.rectTransform.pivot;
            Sprite sprite = Sprite.Create(tex, spriteRect, pivot);
            _image.color = Color.white;
            _image.sprite = sprite;
            _image.gameObject.SetActive(true);
        }
    }
}
