using TMPro;
using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextByTableKey : MonoBehaviour
    {
        [SerializeField] private string _key;
        private TMP_Text _text;
        private ITextManager _textManager;

        [Inject]
        public void Construct(ITextManager textManager)
        {
            _textManager = textManager;
        }

        private void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
            _textManager.OnTextChanged += UpdateText;
            _textManager.OnLanguageChanged += UpdateText;
            UpdateText();
        }

        private void OnDisable()
        {
            _textManager.OnTextChanged -= UpdateText;
            _textManager.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText()
        {
            ChangeText(_key);
        }

        public void ChangeText(string key)
        {
            _key = key;
            SetText(_textManager.GetText(_key));
        }

        private void SetText(string text)
        {
            _text.text = text;
        }
    }
}
