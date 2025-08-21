using System;
using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public abstract class ShowHideAnimation : MonoBehaviour
    {
        public abstract void Show(float delay, float duration, Action callback);
        public abstract void Hide(float delay, float duration, Action callback);
        public abstract void ShowImmediately();
        public abstract void HideImmediately();
    }
}
