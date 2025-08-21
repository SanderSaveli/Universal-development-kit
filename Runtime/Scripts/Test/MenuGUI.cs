using SanderSaveli.UDK.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SanderSaveli.UDK
{
    public class MenuGUI : MonoBehaviour
    {
        [SerializeField] private MainMenuScreenManager _screenManager;
        public void OpenMenu()
        {
            _screenManager.OpenScreen(MenuScreenType.Menu);
        }

        public void OpenSettings()
        {
            _screenManager.OpenScreen(MenuScreenType.Settings);
        }

        public void OpenFAQ()
        {
            _screenManager.OpenScreen(MenuScreenType.FAQ);
        }
    }
}
