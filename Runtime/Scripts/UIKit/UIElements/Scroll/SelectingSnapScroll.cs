using UnityEngine;

namespace SanderSaveli.UDK.UI
{
    public class SelectingSnapScroll : SnapScroll
    {
        private ISelectable _currentSelected;
        private RectTransform[] _children;
        private bool _isSelectionPaused;

        private void Update()
        {
            if (_isSelectionPaused)
                return;

            UpdateCurrentSelection();
        }

        public void SnapToWithoutSelection(RectTransform target)
        {
            _isSelectionPaused = true;
            StopSnapRoutine();
            SnapTo(target);
        }

        public void UpdateCurrentSelection()
        {
            int count = _content.childCount;
            if (count == 0) return;

            if (_children == null || _children.Length != count)
            {
                _children = new RectTransform[count];
                for (int i = 0; i < count; i++)
                    _children[i] = _content.GetChild(i) as RectTransform;
            }

            Vector3 viewportCenterWorld = GetViewportCenter();

            RectTransform nearest = null;
            float minSqrDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Vector3 childCenter = GetWorldCenter(_children[i]);
                float sqrDist = (childCenter - viewportCenterWorld).sqrMagnitude;

                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    nearest = _children[i];
                }
            }

            UpdateSelection(nearest);
        }

        private void UpdateSelection(RectTransform nearest)
        {
            if (nearest == null) return;

            ISelectable selectable = nearest.GetComponent<ISelectable>();
            if (selectable == null) return;

            if (_currentSelected == selectable) return;

            if (_currentSelected != null)
                _currentSelected.Deselect();

            selectable.Select();
            _currentSelected = selectable;
        }

        protected override void OnSnapComplete()
        {
            _isSelectionPaused = false;
        }
    }
}
