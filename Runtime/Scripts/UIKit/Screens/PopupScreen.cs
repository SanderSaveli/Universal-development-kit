using SanderSaveli.UDK.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SanderSaveli.UDK
{
    public class PopupScreen : UiScreen
    {
        [SerializeField] private Button _closeButton;
        protected IScreenCloser _closer;

        [Inject]
        public void Construct(IScreenCloser closer)
        {
            _closer = closer;
        }

        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            _closeButton.onClick.AddListener(HandleClose);
        }

        protected override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
            _closeButton.onClick.RemoveListener(HandleClose);
        }

        private void HandleClose()
        {
            _closer.ClosePopup();
        }
    }
}
