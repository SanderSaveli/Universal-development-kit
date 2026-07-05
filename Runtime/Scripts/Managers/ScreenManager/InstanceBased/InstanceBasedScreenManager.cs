using Cysharp.Threading.Tasks;
using SanderSaveli.UDK.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK.Managers
{
    public class InstanceBasedScreenManager<ScreenType, PopupType> : MonoBehaviour, IScreenManager<ScreenType, PopupType>
        where ScreenType : Enum where PopupType : Enum
    {
        public bool HasOpenedPrefab => _openedPopup != null;

        public UiScreen OpenedScreen => _openedScreen;

        public UiScreen OpenedPopup => _openedPopup;

        [Header("Screens")]
        [SerializeField] protected List<InstanceScreenParams<ScreenType>> Screens = new List<InstanceScreenParams<ScreenType>>();
        [SerializeField] private bool _hasStartScreen;
        [SerializeField] protected ScreenType _startScreen;

        [Header("Popups")]
        [SerializeField] protected List<InstancePopupParams<PopupType>> Popups = new List<InstancePopupParams<PopupType>>();

        protected readonly List<InstancePopupParams<PopupType>> _popupWaitList = new List<InstancePopupParams<PopupType>>();

        protected UiScreen _openedScreen;
        protected UiScreen _openedPopup;

        private void Start()
        {
            if (!_hasStartScreen)
                return;

            InstanceScreenParams<ScreenType> screenParams = GetScreen(_startScreen);

            if (screenParams == null)
            {
                Debug.LogError($"Start screen config not found for type: {_startScreen}.");
                return;
            }

            if (screenParams.Screen == null)
            {
                Debug.LogError($"Start screen instance is null for type: {_startScreen}.");
                return;
            }

            _openedScreen = screenParams.Screen;
            _openedScreen.ShowImmediately();
        }

        public void OpenScreen(ScreenType screenType)
        {
            InstanceScreenParams<ScreenType> screenParams = GetScreen(screenType);

            if (screenParams == null)
            {
                Debug.LogError($"Screen config not found for type: {screenType}.");
                return;
            }

            if (screenParams.Screen == null)
            {
                Debug.LogError($"Screen instance is null for type: {screenType}.");
                return;
            }

            CloseOpenedScreen();
            screenParams.Screen.Show().Forget();
            _openedScreen = screenParams.Screen;
        }

        public void CloseOpenedScreen()
        {
            if (_openedScreen != null)
            {
                _openedScreen.Hide().Forget();
            }

            _openedScreen = null;
        }

        public void AddToPopupQueue(PopupType popup)
        {
            AddToPopupQueueAsync(popup).Forget();
        }

        public void ClosePopup()
        {
            ClosePopupAsync().Forget();
        }

        protected InstanceScreenParams<ScreenType> GetScreen(ScreenType screenType)
        {
            if (Screens == null)
                return null;

            for (int i = 0; i < Screens.Count; i++)
            {
                InstanceScreenParams<ScreenType> screenParams = Screens[i];

                if (screenParams.WindowType.Equals(screenType))
                    return screenParams;
            }

            return null;
        }

        protected InstancePopupParams<PopupType> GetPopup(PopupType popupType)
        {
            if (Popups == null)
                return null;

            for (int i = 0; i < Popups.Count; i++)
            {
                InstancePopupParams<PopupType> popupParams = Popups[i];

                if (popupParams.PopupType.Equals(popupType))
                    return popupParams;
            }

            return null;
        }

        private async UniTask AddToPopupQueueAsync(PopupType popup)
        {
            InstancePopupParams<PopupType> popupParams = GetPopup(popup);

            if (popupParams == null)
            {
                Debug.LogError($"Popup config not found for type: {popup}.");
                return;
            }

            if (popupParams.Screen == null)
            {
                Debug.LogError($"Popup instance is null for type: {popup}.");
                return;
            }

            _popupWaitList.Add(popupParams);
            _popupWaitList.Sort((x, y) => y.Order.CompareTo(x.Order));

            await UniTask.Yield();

            if (_openedPopup == null)
            {
                await OpenNextPopupAsync();
            }
        }

        private async UniTask ClosePopupAsync()
        {
            await UniTask.Yield();

            if (_openedPopup != null)
            {
                UiScreen popup = _openedPopup;
                _openedPopup = null;
                await popup.Hide();
            }

            await OpenNextPopupAsync();
        }

        private async UniTask OpenNextPopupAsync()
        {
            if (_popupWaitList.Count <= 0)
                return;

            InstancePopupParams<PopupType> popupParams = _popupWaitList[0];
            _popupWaitList.RemoveAt(0);

            _openedPopup = popupParams.Screen;
            await _openedPopup.Show();
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Screens != null)
            {
                for (int i = 0; i < Screens.Count; i++)
                {
                    InstanceScreenParams<ScreenType> screen = Screens[i];
                    screen.Name = screen.WindowType.ToString();
                }
            }

            if (Popups != null)
            {
                for (int i = 0; i < Popups.Count; i++)
                {
                    InstancePopupParams<PopupType> popup = Popups[i];
                    popup.Name = popup.PopupType.ToString();
                }
            }
        }
#endif
    }
}
