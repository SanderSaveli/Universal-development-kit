using System;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public abstract class TextsManager<TLanguage, TextTable> : MonoBehaviour, ITextManager, ILanguageChanger<TLanguage> where TLanguage : Enum
    {
        private struct PendingRequest
        {
            public string Key;
            public Action<string> Callback;

            public PendingRequest(string key, Action<string> callback)
            {
                Key = key;
                Callback = callback;
            }
        }

        public TLanguage Language => _language;
        public Action OnLanguageChanged { get; set; }

        [SerializeField] protected string URL = "";
        [SerializeField] protected string Path = "Text/application_texts";
        [SerializeField] private bool _isLocalInBuild = true;

        protected Dictionary<string, TextTable> _tableTexts;

        private List<PendingRequest> _pending = new();
        private TLanguage _language;
        private bool _isTextsLoaded;

        private void Start()
        {
            LoadTexts();
        }

        public void ChangeLanguage(TLanguage locate)
        {
            _language = locate;
            OnLanguageChanged?.Invoke();
        }

        public string GetText(string key, Action<string> lazy)
        {
            if (_isTextsLoaded)
            {
                if (_tableTexts != null && _tableTexts.TryGetValue(key, out var text))
                    return GetCurrentLanguageValue(text);
                Debug.LogError($"[TextsManager] Key '{key}' not found for locale {_language}");
                return key;
            }
            else if(lazy != null)
            {
                _pending.Add(new PendingRequest(key, lazy));
            }
            return "";
        }

        public abstract string GetCurrentLanguageValue(TextTable texts);

        protected void LoadTexts()
        {
            if (_isLocalInBuild)
            {
#if UNITY_EDITOR
                GetTextFromServer(HandleResponce);
#else
                GetTextFromFile(ParseAndResolvePendingRequests);
#endif
            }
            else
            {
                GetTextFromServer(HandleResponce);
            }
        }

        protected abstract void GetTextFromServer(Action<string> callback);

        protected abstract void GetTextFromFile(Action<string> callback);

        protected abstract Dictionary<string, TextTable> ParseResponce(string responce);

        protected abstract void SaveToFile(string data);

        private void HandleResponce(string responce)
        {
            ParseAndResolvePendingRequests(responce);
            if (_isLocalInBuild)
            {
                SaveToFile(responce);
            }
        }

        private void ParseAndResolvePendingRequests(string responce)
        {
            _isTextsLoaded = true;
            _tableTexts = ParseResponce(responce);

            if (_pending != null && _pending.Count > 0)
            {
                foreach (PendingRequest request in _pending)
                {
                    request.Callback?.Invoke(GetText(request.Key, null));
                }
            }
            _pending.Clear();
        }
    }
}