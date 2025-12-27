using System;
using UnityEngine;

namespace SanderSaveli.GravityMaze
{
    public class InputManager : MonoBehaviour, IInputManager
    {
        [Tooltip("Threshold in pixels - if the movement is greater, it is considered drag")]
        [SerializeField] private float _dragThresholdPixels = 10f;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_pointerDown)
                {
                    ProcessPointerUp(_currPos);
                }
                _isEnabled = value;
            }
        }
        public Action<Vector2> OnPointerDown { get; set; }
        public Action<Vector2> OnPointerUp { get; set; }
        public Action<Vector2> OnClick { get; set; }
        public Action<Vector2> OnBeginDrag { get; set; }
        public Action<Vector2> OnDrag { get; set; }
        public Action<Vector2> OnEndDrag { get; set; }

        private bool _isEnabled;
        private bool _pointerDown;
        private bool _isDragging;
        private Vector2 _startPos;
        private Vector2 _currPos;
        private int _activeTouchId = -1;

        private void Awake()
        {
            IsEnabled = true;
        }

        private void Update()
        {
            if (Time.timeScale == 0 || !IsEnabled)
            {
                return;
            }
            HandleMouse();
            HandleTouch();
        }

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos = Input.mousePosition;
                ProcessPointerDown(pos);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 pos = Input.mousePosition;
                ProcessPointerMove(pos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 pos = Input.mousePosition;
                ProcessPointerUp(pos);
            }
        }

        private void HandleTouch()
        {
            if (Input.touchCount == 0) return;

            Touch touch = Input.GetTouch(0);
            Vector2 pos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _activeTouchId = touch.fingerId;
                    ProcessPointerDown(pos);
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (touch.fingerId == _activeTouchId)
                        ProcessPointerMove(pos);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == _activeTouchId)
                    {
                        ProcessPointerUp(pos);
                        _activeTouchId = -1;
                    }
                    break;
            }
        }

        private void ProcessPointerDown(Vector2 screenPos)
        {
            _pointerDown = true;
            _isDragging = false;
            _startPos = screenPos;
            _currPos = screenPos;
            OnPointerDown?.Invoke(screenPos);
        }

        private void ProcessPointerMove(Vector2 screenPos)
        {
            if (!_pointerDown) return;
            _currPos = screenPos;
            float moved = Vector2.Distance(screenPos, _startPos);

            if (!_isDragging && moved > _dragThresholdPixels)
            {
                _isDragging = true;
                OnBeginDrag?.Invoke(_startPos);
            }

            if (_isDragging)
            {
                OnDrag?.Invoke(screenPos);
            }
        }

        private void ProcessPointerUp(Vector2 screenPos)
        {
            if (!_pointerDown) return;
            _currPos = screenPos;
            if (_isDragging)
            {
                OnEndDrag?.Invoke(screenPos);
            }
            else
            {
                OnClick?.Invoke(screenPos);
            }

            OnPointerUp?.Invoke(screenPos);

            _pointerDown = false;
            _isDragging = false;
            _activeTouchId = -1;
        }
    }

}
