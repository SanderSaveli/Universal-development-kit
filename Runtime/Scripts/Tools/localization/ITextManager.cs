using System;

namespace SanderSaveli.UDK
{
    public interface ITextManager
    {
        public Action OnLanguageChanged { get; set; }
        public string GetText(string key, Action<string> lazy);
    }
}