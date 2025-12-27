using System;
using UnityEngine;

namespace SanderSaveli.GravityMaze
{
    public interface IInputManager
    {
        public bool IsEnabled { get; set; }

        public Action<Vector2> OnPointerDown { get; set; }
        public Action<Vector2> OnPointerUp { get; set; }
        public Action<Vector2> OnClick { get; set; }
        public Action<Vector2> OnBeginDrag { get; set; }
        public Action<Vector2> OnDrag { get; set; }
        public Action<Vector2> OnEndDrag { get; set; }
    }
}
