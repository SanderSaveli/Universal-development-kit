using System;

namespace SanderSaveli.UDK.UI
{
    public interface ISelectable
    {
        public bool IsSelected { get; }
        public Action<bool> OnSelectChange { get; set; }

        public void Select();
        public void Deselect();
    }
}
