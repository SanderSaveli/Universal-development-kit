using SanderSaveli.UDK.UI;
using System;

namespace SanderSaveli.UDK.Managers
{
    public interface IScreenManager<ScreenType, PopupType> : IScreenCloser where ScreenType : Enum where PopupType : Enum
    {
        public bool HasOpenedPrefab {  get; }
        public UiScreen OpenedScreen { get; }
        public UiScreen OpenedPopup { get; }

        public void OpenScreen(ScreenType screenType);

        public void AddToPopupQueue(PopupType popup);
    }
}
