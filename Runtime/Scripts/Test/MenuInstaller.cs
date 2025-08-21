using SanderSaveli.UDK.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SanderSaveli.UDK
{
    public class MenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.DeclareSignal<SignalInputOpenMenuScreen>();
            Container.DeclareSignal<SignalInputOpenMenuPopup>();
            Container.DeclareSignal<SignalInputClosePopup>();

        }
    }
}
