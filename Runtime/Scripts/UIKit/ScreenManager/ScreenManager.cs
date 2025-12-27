using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    [Serializable]
    public class ScreenParams<T>
    {
        [HideInInspector] public string Name;
        public T WindowType;
        public UiScreen Screen;
    }

    [Serializable]
    public class PopupParams<T>
    {
        [HideInInspector] public string Name;
        public T PopupType;
        public UiScreen Screen;
        public int Order;
    }

    public class ScreenManager<ScreenType, PopupType> : MonoBehaviour where ScreenType: Enum where PopupType: Enum
    {
        [SerializeField] protected List<ScreenParams<ScreenType>> Screens = new();
        [SerializeField] protected List<PopupParams<PopupType>> Popups = new();
        [SerializeField] private bool _hasStartScreen;
        [SerializeField] protected ScreenType _startScreen;

        protected UiScreen _openedScreen;
        protected UiScreen _openedPopup;

        protected List<PopupParams<PopupType>> _popupWaitList = new();

        private void Start()
        {
            if(_hasStartScreen)
            {
                _openedScreen = GetScreen(_startScreen).Screen;
                _openedScreen.ShowImmediately();
            }
        }

        public void OpenScreen(ScreenType screenType)
        {
            ScreenParams<ScreenType> screenParams = GetScreen(screenType);
            CloseOpenedScreen();
            screenParams.Screen.Show();
            _openedScreen = screenParams.Screen;
        }

        public void CloseOpenedScreen()
        {
            _openedScreen?.Hide();
            _openedScreen = null;
        }

        public async void AddToPopupQueue(PopupType popup)
        {
            PopupParams<PopupType> popupParams = GetPopup(popup);

            _popupWaitList.Add(popupParams);
            _popupWaitList.Sort((x, y) => y.Order.CompareTo(x.Order));
            await Task.Yield();
            if (_openedPopup == null)
            {
                OpenNextPopup();
            }
        }

        public async void ClosePopup()
        {
            await Task.Yield();
            if (_openedPopup != null)
            {
                _openedPopup.Hide();
                _openedPopup = null;
            }
            OpenNextPopup();
        }

        private void OpenNextPopup()
        {
            if (_popupWaitList.Count <= 0)
                return;

            PopupParams<PopupType> popup = _popupWaitList[0];
            _popupWaitList.RemoveAt(0);
            _openedPopup = popup.Screen;
            _openedPopup.Show();
        }

        protected ScreenParams<ScreenType> GetScreen(ScreenType screenType) =>
            Screens.FirstOrDefault(t => t.WindowType.Equals(screenType));

        protected PopupParams<PopupType> GetPopup(PopupType screenType) =>
            Popups.FirstOrDefault(t => t.PopupType.Equals(screenType));

#if UNITY_EDITOR
        protected void OnValidate()
        {
            foreach (var window in Screens)
            {
                window.Name = window.WindowType.ToString();
            }

            foreach (var popup in Popups)
            {
                popup.Name = popup.PopupType.ToString();
            }
        }
#endif
    }
}
