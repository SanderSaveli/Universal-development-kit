using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK.UI
{
    public class ExpandableMenuItem : MonoBehaviour
    {
        public GameObject DescriptionPanel { get; set; }
        public Transform ExpandArrow { get; set; }
        public Button ToggleButton { get; set; }

        [Space]
        [SerializeField] protected float _animationDuration = 0.3f;
        [SerializeField] private bool _collapsedAtStart = true;

        private RectTransform _rectTransform;
        private ContentSizeFitter _contentSizeFitter;
        private bool _isExpanded = false;

        private float _expandedHeight;
        private float _collapsedHeight;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _contentSizeFitter = GetComponent<ContentSizeFitter>();
            if (GetComponent<RectMask2D>() == null)
            {
                gameObject.AddComponent<RectMask2D>();
            }

            _contentSizeFitter.enabled = true;
            DescriptionPanel.SetActive(!_collapsedAtStart);
        }

        private void Start()
        {
            if(ToggleButton == null)
            {
                ToggleButton = gameObject.GetComponent<Button>();
            }
            ToggleButton.onClick.AddListener(Toggle);
        }

        public void Toggle()
        {
            if (_isExpanded)
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }

        protected virtual void Expand()
        {
            if (_collapsedHeight == 0)
            {
                _collapsedHeight = _rectTransform.rect.height;
            }

            _isExpanded = true;

            DescriptionPanel.SetActive(true);
            _contentSizeFitter.enabled = false;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

            _expandedHeight = LayoutUtility.GetPreferredHeight(_rectTransform);

            _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, _expandedHeight), _animationDuration)
                         .SetEase(Ease.InOutQuad)
                         .OnComplete(() => _contentSizeFitter.enabled = true);
            if (ExpandArrow != null)
            {
                ExpandArrow.DORotate(new Vector3(0, 0, 180), _animationDuration);
            }

        }

        protected virtual void Collapse()
        {
            _isExpanded = false;

            _contentSizeFitter.enabled = false;

            _rectTransform.DOSizeDelta(new Vector2(_rectTransform.sizeDelta.x, _collapsedHeight), _animationDuration)
                         .SetEase(Ease.InOutQuad)
                         .OnComplete((TweenCallback)(() =>
                         {
                             this.DescriptionPanel.SetActive(false);
                             _contentSizeFitter.enabled = true;
                         }));
            if (ExpandArrow != null)
            {
                ExpandArrow.DORotate(Vector3.zero, _animationDuration);
            }
        }
    }
}
