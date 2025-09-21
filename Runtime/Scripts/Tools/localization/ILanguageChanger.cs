namespace SanderSaveli.UDK
{
    public interface ILanguageChanger<TLanguage>
    {
        public TLanguage Language { get; }
        public void ChangeLanguage(TLanguage language);
    }
}
