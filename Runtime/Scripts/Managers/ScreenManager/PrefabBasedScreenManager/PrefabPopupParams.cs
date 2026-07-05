using SanderSaveli.UDK.UI;
using System;
using UnityEngine;

namespace SanderSaveli.UDK.Managers
{
    [Serializable]
    public class PrefabPopupParams<T>
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public T PopupType => _popupType;
        public UiScreen Prefab => _prefab;
        public int Order => _order;

        [HideInInspector][SerializeField] private string _name;
        [SerializeField] private T _popupType;
        [SerializeField] private UiScreen _prefab;
        [SerializeField] private int _order;
    }
}
