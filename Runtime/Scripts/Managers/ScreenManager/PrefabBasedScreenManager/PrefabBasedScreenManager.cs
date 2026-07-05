using Cysharp.Threading.Tasks;
using SanderSaveli.UDK.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK.Managers
{
    public class PrefabBasedScreenManager<ScreenType, PopupType> : MonoBehaviour, IScreenManager<ScreenType, PopupType>
        where ScreenType : Enum where PopupType : Enum
    {
        public bool HasOpenedPrefab => _hasOpenedPopup;

        public UiScreen OpenedScreen => _openedScreen;

        public UiScreen OpenedPopup => _openedPopup;

        [Header("Components")]
        [SerializeField] private Transform _screenRoot;
        [SerializeField] private Transform _popupRoot;

        [Header("Screens")]
        [SerializeField] protected List<PrefabScreenParams<ScreenType>> Screens = new List<PrefabScreenParams<ScreenType>>();
        [SerializeField] private bool _hasStartScreen;
        [SerializeField] protected ScreenType _startScreen;

        [Header("Popups")]
        [SerializeField] protected List<PrefabPopupParams<PopupType>> Popups = new List<PrefabPopupParams<PopupType>>();

        private readonly Dictionary<ScreenType, UiScreen> _spawnedScreens = new Dictionary<ScreenType, UiScreen>();
        private readonly Dictionary<PopupType, UiScreen> _spawnedPopups = new Dictionary<PopupType, UiScreen>();

        protected readonly List<PrefabPopupParams<PopupType>> _popupWaitList = new List<PrefabPopupParams<PopupType>>();

        protected UiScreen _openedScreen;
        protected UiScreen _openedPopup;

        private DiContainer _container;
        private PopupType _openedPopupType;
        private bool _hasOpenedPopup;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Start()
        {
            if (!_hasStartScreen)
                return;

            UiScreen startScreen = GetOrCreateScreen(_startScreen);

            if (startScreen == null)
                return;

            _openedScreen = startScreen;
            _openedScreen.ShowImmediately();
        }

        public void OpenScreen(ScreenType screenType)
        {
            OpenScreenAsync(screenType).Forget();
        }

        public void CloseOpenedScreen()
        {
            CloseOpenedScreenAsync().Forget();
        }

        public void AddToPopupQueue(PopupType popup)
        {
            AddToPopupQueueAsync(popup).Forget();
        }

        public void ClosePopup()
        {
            ClosePopupAsync().Forget();
        }

        protected PrefabScreenParams<ScreenType> GetScreen(ScreenType screenType)
        {
            if (Screens == null)
                return null;

            for (int i = 0; i < Screens.Count; i++)
            {
                PrefabScreenParams<ScreenType> screenParams = Screens[i];

                if (screenParams.WindowType.Equals(screenType))
                    return screenParams;
            }

            return null;
        }

        protected PrefabPopupParams<PopupType> GetPopup(PopupType popupType)
        {
            if (Popups == null)
                return null;

            for (int i = 0; i < Popups.Count; i++)
            {
                PrefabPopupParams<PopupType> popupParams = Popups[i];

                if (popupParams.PopupType.Equals(popupType))
                    return popupParams;
            }

            return null;
        }

        private async UniTask OpenScreenAsync(ScreenType screenType)
        {
            UiScreen screen = GetOrCreateScreen(screenType);

            if (screen == null)
                return;

            await CloseOpenedScreenAsync();

            _openedScreen = screen;
            await _openedScreen.Show();
        }

        private async UniTask CloseOpenedScreenAsync()
        {
            if (_openedScreen == null)
                return;

            UiScreen screen = _openedScreen;
            _openedScreen = null;
            await screen.Hide();
        }

        private async UniTask AddToPopupQueueAsync(PopupType popup)
        {
            PrefabPopupParams<PopupType> popupParams = GetPopup(popup);

            if (popupParams == null)
            {
                Debug.LogError($"Popup config not found for type: {popup}.");
                return;
            }

            if (popupParams.Prefab == null)
            {
                Debug.LogError($"Popup prefab is null for type: {popup}.");
                return;
            }

            if (IsPopupQueuedOrOpened(popup))
                return;

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
                _hasOpenedPopup = false;
                await popup.Hide();
            }

            await OpenNextPopupAsync();
        }

        private async UniTask OpenNextPopupAsync()
        {
            if (_popupWaitList.Count <= 0)
                return;

            PrefabPopupParams<PopupType> popupParams = _popupWaitList[0];
            _popupWaitList.RemoveAt(0);

            UiScreen popup = GetOrCreatePopup(popupParams.PopupType);

            if (popup == null)
                return;

            _openedPopup = popup;
            _openedPopupType = popupParams.PopupType;
            _hasOpenedPopup = true;
            await _openedPopup.Show();
        }

        private UiScreen GetOrCreateScreen(ScreenType screenType)
        {
            if (_spawnedScreens.TryGetValue(screenType, out UiScreen cachedScreen))
                return cachedScreen;

            PrefabScreenParams<ScreenType> screenParams = GetScreen(screenType);

            if (screenParams == null)
            {
                Debug.LogError($"Screen config not found for type: {screenType}.");
                return null;
            }

            if (screenParams.Prefab == null)
            {
                Debug.LogError($"Screen prefab is null for type: {screenType}.");
                return null;
            }

            if (_screenRoot == null)
            {
                Debug.LogError("Screen root is null.");
                return null;
            }

            if (_container == null)
            {
                Debug.LogError("DiContainer is null.");
                return null;
            }

            UiScreen screen = _container.InstantiatePrefabForComponent<UiScreen>(screenParams.Prefab, _screenRoot);
            screen.HideImmediately();
            _spawnedScreens.Add(screenType, screen);

            return screen;
        }

        private UiScreen GetOrCreatePopup(PopupType popupType)
        {
            if (_spawnedPopups.TryGetValue(popupType, out UiScreen cachedPopup))
                return cachedPopup;

            PrefabPopupParams<PopupType> popupParams = GetPopup(popupType);

            if (popupParams == null)
            {
                Debug.LogError($"Popup config not found for type: {popupType}.");
                return null;
            }

            if (popupParams.Prefab == null)
            {
                Debug.LogError($"Popup prefab is null for type: {popupType}.");
                return null;
            }

            if (_popupRoot == null)
            {
                Debug.LogError("Popup root is null.");
                return null;
            }

            if (_container == null)
            {
                Debug.LogError("DiContainer is null.");
                return null;
            }

            UiScreen popup = _container.InstantiatePrefabForComponent<UiScreen>(popupParams.Prefab, _popupRoot);
            popup.HideImmediately();
            _spawnedPopups.Add(popupType, popup);

            return popup;
        }

        private bool IsPopupQueuedOrOpened(PopupType popupType)
        {
            if (_hasOpenedPopup && _openedPopupType.Equals(popupType))
                return true;

            for (int i = 0; i < _popupWaitList.Count; i++)
            {
                PrefabPopupParams<PopupType> popupParams = _popupWaitList[i];

                if (popupParams.PopupType.Equals(popupType))
                    return true;
            }

            return false;
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Screens != null)
            {
                for (int i = 0; i < Screens.Count; i++)
                {
                    PrefabScreenParams<ScreenType> screen = Screens[i];
                    screen.Name = screen.WindowType.ToString();
                }
            }

            if (Popups != null)
            {
                for (int i = 0; i < Popups.Count; i++)
                {
                    PrefabPopupParams<PopupType> popup = Popups[i];
                    popup.Name = popup.PopupType.ToString();
                }
            }
        }
#endif
    }
}
