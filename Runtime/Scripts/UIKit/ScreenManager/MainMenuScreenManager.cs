using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK.UI
{
    public class MainMenuScreenManager : ScreenManager<MenuScreenType, MenuPopupType>
    {
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void OnEnable()
        {
            _signalBus.Subscribe<SignalInputOpenMenuScreen>(OnInputOpenWindow);
            _signalBus.Subscribe<SignalInputOpenMenuPopup>(OnInputOpenPopup);
            _signalBus.Subscribe<SignalInputClosePopup>(HandleClosePopup);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<SignalInputOpenMenuScreen>(OnInputOpenWindow);
            _signalBus.Unsubscribe<SignalInputOpenMenuPopup>(OnInputOpenPopup);
            _signalBus.TryUnsubscribe<SignalInputClosePopup>(HandleClosePopup);
        }

        private void OnInputOpenWindow(SignalInputOpenMenuScreen ctx)
        {
            OpenScreen(ctx.WindowType);
        }

        private void OnInputOpenPopup(SignalInputOpenMenuPopup ctx)
        {
            AddToPopupQueue(ctx.Popup);
        }

        private void HandleClosePopup(SignalInputClosePopup ctx)
        {
            ClosePopup();
        }
    }
}
