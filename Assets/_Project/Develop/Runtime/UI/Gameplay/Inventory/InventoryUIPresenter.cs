using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Inventory
{
    public class InventoryUIPresenter : EntityView
    {
        [Header("Main Containers")]
        [SerializeField] private CanvasGroup _mainCanvasGroup;
        [SerializeField] private GameObject _itemAnimateShow;
        [SerializeField] private GameObject _itemAnimateHide;
        [SerializeField] private GameObject _itemFinalView;

        [Header("Final View Components")]
        [SerializeField] private Image _itemIconImage;
        [SerializeField] private TextMeshProUGUI _countText;

        [Header("Animators")]
        [SerializeField] private Animator _showAnimator;
        [SerializeField] private Animator _hideAnimator;

        [Header("Settings")]
        [SerializeField] private float _fadeDuration = 0.15f;
        [SerializeField] private float _displayDuration = 1.5f;
        [SerializeField] private float _animDelay = 0.25f; // Уменьшил для отзывчивости
        [SerializeField] private ThrowableConfig[] _consumables;

        private Sequence _fadeSequence;
        private Sequence _switchSequence;
        private Entity _entity;
        private List<IDisposable> _chargeSubscriptions = new();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;

            _mainCanvasGroup.alpha = 0;
            ResetToIdleState();

            _chargeSubscriptions.Add(_entity.ShurikenCharges.Subscribe((old, val) => RefreshCountText()));
            _chargeSubscriptions.Add(_entity.SleepDartCharges.Subscribe((old, val) => RefreshCountText()));

            _entity.CurrentThrowableIndex.Subscribe(OnItemSwitched);
        }

        private void OnItemSwitched(int oldIdx, int newIdx)
        {
            UpdateIcon(newIdx);
            RefreshCountText();
            ShowInventory();
            PlaySwitchAnimation();
        }

        private void ShowInventory()
        {
            // Управляем прозрачностью отдельно
            _fadeSequence?.Kill();
            _fadeSequence = DOTween.Sequence();

            _fadeSequence.Append(_mainCanvasGroup.DOFade(1, _fadeDuration));
            _fadeSequence.AppendInterval(_displayDuration);
            _fadeSequence.Append(_mainCanvasGroup.DOFade(0, _fadeDuration));
            _fadeSequence.OnComplete(ResetToIdleState);
        }

        private void PlaySwitchAnimation()
        {
            _switchSequence?.Kill();
            _switchSequence = DOTween.Sequence();

            // Мгновенная подготовка к новой анимации
            _switchSequence.AppendCallback(() =>
            {
                _itemFinalView.SetActive(false);

                // Перезапускаем стейты аниматоров, чтобы они не залипали
                _itemAnimateShow.SetActive(false);
                _itemAnimateHide.SetActive(false);
                _itemAnimateShow.SetActive(true);
                _itemAnimateHide.SetActive(true);

                _showAnimator.Play("ShowItem", -1, 0f); // Прямой запуск стейта вместо триггера
                _hideAnimator.Play("HideItem", -1, 0f);
            });

            _switchSequence.AppendInterval(_animDelay);

            _switchSequence.AppendCallback(() =>
            {
                _itemAnimateShow.SetActive(false);
                _itemAnimateHide.SetActive(false);
                _itemFinalView.SetActive(true);
            });
        }

        private void ResetToIdleState()
        {
            _itemFinalView.SetActive(false);
            _itemAnimateShow.SetActive(false);
            _itemAnimateHide.SetActive(false);
        }

        private void UpdateIcon(int index) => _itemIconImage.sprite = _consumables[index].Icon;

        private void RefreshCountText()
        {
            int index = _entity.CurrentThrowableIndex.Value;
            int charges = index == 0 ? _entity.ShurikenCharges.Value : _entity.SleepDartCharges.Value;
            _countText.text = charges.ToString();
        }

        public override void Cleanup(Entity entity)
        {
            if (this == null) return;

            base.Cleanup(entity);
            foreach (var sub in _chargeSubscriptions) sub.Dispose();
            _fadeSequence?.Kill();
            _switchSequence?.Kill();
        }
    }
}