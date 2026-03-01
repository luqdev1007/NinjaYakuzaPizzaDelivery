using DG.Tweening;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.UI.Core;

namespace Assets._Project.Develop.Runtime.Gameplay.UI
{
    public class ProgressFilledImageView : MonoBehaviour, IShowableView
    {
        [SerializeField] private Image _filledImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private float _duration = 0.25f;

        [Header("Configs")]
        [SerializeField] private Sprite[] _icons;

        private IReadOnlyVariable<float> _currentValue;
        private IReadOnlyVariable<float> _maxValue;

        private Dictionary<Sprite, float> _iconProgress;

        private Tween _fillTween;

        public void Init(IReadOnlyVariable<float> currentValue, IReadOnlyVariable<float> maxValue)
        {
            _iconProgress = new Dictionary<Sprite, float>
            {
                { _icons[0], 0.9f },
                { _icons[1], 0.5f },
                { _icons[2], 0.15f },
            };

            _currentValue = currentValue;
            _maxValue = maxValue;

            UpdateFillAmount(true);
            CheckUpdateIcon();

            _currentValue.Subscribe(OnProgressChanged);
            _maxValue.Subscribe(OnProgressChanged);
        }

        private void OnProgressChanged(float oldValue, float newValue)
        {
            UpdateFillAmount(false);
        }

        private void UpdateFillAmount(bool immediate)
        {
            float targetFill = _currentValue.Value / _maxValue.Value;

            if (immediate)
            {
                _filledImage.fillAmount = targetFill;
                return;
            }

            _fillTween?.Kill();

            _fillTween = _filledImage.DOFillAmount(targetFill, _duration)
                .SetEase(Ease.OutQuad).OnComplete(CheckUpdateIcon);
        }

        private void CheckUpdateIcon()
        {
            foreach (KeyValuePair<Sprite, float> i in _iconProgress)
            {
                if (_filledImage.fillAmount >= i.Value)
                {
                    _iconImage.sprite = i.Key;
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            _fillTween?.Kill();
        }

        public Tween Hide()
        {
            gameObject.SetActive(false);
            transform.DOKill();

            return DOTween.Sequence();
        }

        public Tween Show()
        {
            gameObject.SetActive(true);
            transform.DOKill();

            return transform
                .DOScale(1, 0.1f)
                .From(0)
                .SetUpdate(true)
                .Play();
        }
    }
}