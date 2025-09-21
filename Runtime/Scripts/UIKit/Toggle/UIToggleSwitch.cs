using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SanderSaveli.UDK
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("ToggleSwitch")]
    public class UIToggleSwitch : MonoBehaviour
    {
        public Action<bool> OnToggle;
        public bool Value => _isOn;

        public RectTransform Handler;
        public bool IsOnAtStart;

        [Header("Animation Settings")]
        public float AnimateTime = 0.2f;
        public Ease AnimateType = Ease.Linear;
        public Color ColorOn = Color.green;
        public Color ColorOff = Color.gray;
        public float AncorMinX = 5f;
        public float AncorMaxX = -5f;

        private Button _button;
        private Image _image;
        private bool _isOn;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            SetValueImmediately(IsOnAtStart);
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Toggle);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Toggle);
        }

        private void Toggle()
        {
            SetValue(!_isOn);
        }

        public void SetValue(bool isOn)
        {
            _isOn = isOn;
            _image.DOKill();
            Handler.DOKill();

            Handler.DOAnchorMin(new Vector2(isOn ? 1 : 0, 0.5f), AnimateTime).SetEase(AnimateType);
            Handler.DOAnchorMax(new Vector2(isOn ? 1 : 0, 0.5f), AnimateTime).SetEase(AnimateType);

            Handler.DOPivotX(isOn ? 1 : 0, AnimateTime).SetEase(AnimateType);

            _image.DOColor(isOn ? ColorOn : ColorOff, AnimateTime).SetEase(AnimateType);
            Handler.DOAnchorPosX(isOn ? AncorMaxX : AncorMinX, AnimateTime).SetEase(AnimateType);

            OnToggle?.Invoke(isOn);
        }

        public void SetValueImmediately(bool isOn)
        {
            _isOn = isOn;
            _image.DOKill();
            Handler.DOKill();

            Handler.anchorMax = new Vector2(isOn ? 1 : 0, 0.5f);
            Handler.anchorMin = new Vector2(isOn ? 1 : 0, 0.5f);

            Handler.pivot = new Vector2(isOn ? 1 : 0, 0.5f);

            _image.color = isOn ? ColorOn : ColorOff;
            Handler.anchoredPosition = 
                new Vector2(isOn ? AncorMaxX : AncorMinX, Handler.anchoredPosition.y);

            OnToggle?.Invoke(isOn);
        }

        private void OnDestroy()
        {
            _image.DOKill();
            Handler.DOKill();
        }
    }
}
