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
            _signalBus.Subscribe<SignalInputOpenMenuScreen>(OnInputOpenScreen);
            _signalBus.Subscribe<SignalInputOpenMenuPopup>(OnInputOpenPopup);
            _signalBus.Subscribe<SignalInputClosePopup>(HandleClosePopup);
            _signalBus.Subscribe<SignalInputCloseScreen>(CloseOpenedScreen);
        }

        private void OnDisable()
        {
            _signalBus.Unsubscribe<SignalInputOpenMenuScreen>(OnInputOpenScreen);
            _signalBus.Unsubscribe<SignalInputOpenMenuPopup>(OnInputOpenPopup);
            _signalBus.TryUnsubscribe<SignalInputClosePopup>(HandleClosePopup);
            _signalBus.TryUnsubscribe<SignalInputCloseScreen>(CloseOpenedScreen);
        }

        private void OnInputOpenScreen(SignalInputOpenMenuScreen ctx)
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
