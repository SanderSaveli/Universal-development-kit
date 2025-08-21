namespace SanderSaveli.UDK.UI
{
    public readonly struct SignalInputOpenMenuScreen
    {
        public readonly MenuScreenType WindowType;

        public SignalInputOpenMenuScreen(MenuScreenType windowType)
        {
            WindowType = windowType;
        }
    }
}
