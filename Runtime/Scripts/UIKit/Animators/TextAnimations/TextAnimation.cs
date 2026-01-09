using TMPro;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public abstract class TextAnimation : MonoBehaviour
    {
        [SerializeField] protected TMP_Text _textField;
        [Min(0)]
        [SerializeField] private float _duration = 0.5f;

        private void Reset()
        {
            _textField = gameObject.GetComponent<TMP_Text>();
        }

        public void ChangeText(string text)
        {
            AnimateText(text, _textField, _duration);
        }

        public void ChangeText(string text, float duration)
        {
            AnimateText(text, _textField, duration);
        }


        public void ChangeTextWithoutAnimation(string text)
        {
            _textField.text = text;
        }

        protected abstract void AnimateText(string text, TMP_Text field, float duration);
    }
}
