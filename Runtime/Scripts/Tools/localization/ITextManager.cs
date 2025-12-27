using System;

namespace SanderSaveli.UDK
{
    public interface ITextManager
    {
        public Action OnLanguageChanged { get; set; }
        public Action OnTextChanged { get; set; }
        public string GetText(string key);
    }
}