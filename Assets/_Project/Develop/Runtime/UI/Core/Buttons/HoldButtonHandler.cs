using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Core.Buttons
{
    [RequireComponent(typeof(Button))]
    public class HoldButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnHoldComplete;

        [Header("Settings")]
        [SerializeField] private float _holdDuration = 1.0f;
        [SerializeField] private float _resetSpeed = 2.0f;
        [SerializeField] private Image _progressImage;

        private bool _isPressed;
        private float _currentProgress;

        private void OnDisable()
        {
            ResetProgress();
        }

        private void Update()
        {
            if (_isPressed)
            {
                _currentProgress += Time.deltaTime / _holdDuration;
                if (_currentProgress >= 1f)
                {
                    CompleteHold();
                }
            }
            else if (_currentProgress > 0)
            {
                _currentProgress -= Time.deltaTime * _resetSpeed;
            }

            _currentProgress = Mathf.Clamp01(_currentProgress);
            UpdateUI();
        }

        public void OnPointerDown(PointerEventData eventData) => _isPressed = true;
        public void OnPointerUp(PointerEventData eventData) => _isPressed = false;

        private void CompleteHold()
        {
            _isPressed = false;
            _currentProgress = 0;
            UpdateUI();
            OnHoldComplete?.Invoke();
        }

        private void ResetProgress()
        {
            _isPressed = false;
            _currentProgress = 0;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_progressImage != null)
                _progressImage.fillAmount = _currentProgress;
        }
    }
}
