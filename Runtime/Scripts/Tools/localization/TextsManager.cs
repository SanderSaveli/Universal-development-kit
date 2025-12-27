using System;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public abstract class TextsManager<TLanguage, TextTable> : MonoBehaviour, ITextManager, ILanguageChanger<TLanguage> where TLanguage : Enum
    {
        public TLanguage Language => _language;
        public Action OnLanguageChanged { get; set; }
        public Action OnTextChanged { get; set; }

        [SerializeField] private bool _isLocalInBuild = true;

        protected Dictionary<string, TextTable> _tableTexts;
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

        public string GetText(string key)
        {
            if (_isTextsLoaded)
            {
                if (_tableTexts != null && _tableTexts.TryGetValue(key, out var text))
                    return GetCurrentLanguageValue(text);
                Debug.LogError($"[TextsManager] Key '{key}' not found for locale {_language}");
                return key;
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
                GetTextFromFile(HandleLoadFromFile);
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
            UpdateTexts(responce);
            if (_isLocalInBuild)
            {
                SaveToFile(responce);
            }
        }

        private void UpdateTexts(string text)
        {
            _isTextsLoaded = true;
            _tableTexts = ParseResponce(text);
            OnTextChanged?.Invoke();
        }
    }
}