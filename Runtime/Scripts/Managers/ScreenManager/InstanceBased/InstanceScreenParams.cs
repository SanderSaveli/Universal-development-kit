using SanderSaveli.UDK.UI;
using System;
using UnityEngine;

namespace SanderSaveli.UDK.Managers
{
    [Serializable]
    public class InstanceScreenParams<T>
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public T WindowType => _windowType;
        public UiScreen Screen => _screen;

        [HideInInspector][SerializeField] private string _name;
        [SerializeField] private T _windowType;
        [SerializeField] private UiScreen _screen;
    }

}
