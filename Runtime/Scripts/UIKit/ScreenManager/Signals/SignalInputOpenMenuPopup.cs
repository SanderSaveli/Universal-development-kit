namespace SanderSaveli.UDK.UI
{
    public readonly struct SignalInputOpenMenuPopup
    {
        public readonly MenuPopupType Popup;

        public SignalInputOpenMenuPopup(MenuPopupType popup)
        {
            Popup = popup;
        }
    }
}
