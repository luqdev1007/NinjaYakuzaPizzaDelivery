using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Inventory
{
    public class InventoryUIPresenter : EntityView, IRequireAudioService
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
        [SerializeField] private float _animDelay = 0.25f;
        [SerializeField] private ThrowableItemConfig[] _consumables;

        [Header("Shake Settings (Empty Use)")]
        [SerializeField] private float _shakeDuration = 0.4f; 
        [SerializeField] private float _shakeStrength = 12f;  

        [Header("SFX Keys")]
        [SerializeField] private string _switchItemSfxKey = "InventorySwitch";

        private Sequence _fadeSequence;
        private Sequence _switchSequence;
        private Tween _shakeTween;

        private Entity _entity;
        private List<IDisposable> _chargeSubscriptions = new();
        private IDisposable _itemIntentDisposable;
        private IAudioService _audioService;

        private Vector3 _originalFinalViewPosition; 

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;

            _originalFinalViewPosition = _itemFinalView.transform.localPosition;

            _mainCanvasGroup.alpha = 0;
            ResetToIdleState();

            var charges = _entity.InventoryCharges;
            for (int i = 0; i < charges.Count; i++)
            {
                _chargeSubscriptions.Add(charges[i].Subscribe((old, val) => RefreshCountText()));
            }

            _entity.CurrentItemIndex.Subscribe(OnItemSwitched);

            _itemIntentDisposable = _entity.IntentUseItem.Subscribe(OnUseAttempt);

            RefreshCountText();
        }

        private void OnUseAttempt(bool oldValue, bool newValue)
        {
            if (_entity == null || newValue == false)
                return;

            int index = _entity.CurrentItemIndex.Value;
            var chargesList = _entity.InventoryCharges;

            if (chargesList != null && index >= 0 && index < chargesList.Count)
            {
                if (chargesList[index].Value <= 0)
                {
                    ShowInventory(); 
                    PlayEmptyShake(); 
                }
            }
        }

        private void PlayEmptyShake()
        {
            _itemFinalView.SetActive(true);

            if (_shakeTween != null && _shakeTween.IsActive())
            {
                _shakeTween.Kill(true);
            }
            _itemFinalView.transform.localPosition = _originalFinalViewPosition;

            Vector3 shakeVector = new Vector3(_shakeStrength, 0f, 0f);

            _shakeTween = _itemFinalView.transform.DOShakePosition(
                duration: _shakeDuration,
                strength: shakeVector,
                vibrato: 14,
                randomness: 90,
                snapping: false,
                fadeOut: true
            ).OnComplete(() =>
            {
                _itemFinalView.transform.localPosition = _originalFinalViewPosition;
            });
        }

        private void OnItemSwitched(int oldIdx, int newIdx)
        {
            _shakeTween?.Kill(true);
            _itemFinalView.transform.localPosition = _originalFinalViewPosition;

            UpdateIcon(newIdx);
            RefreshCountText();
            ShowInventory();
            PlaySwitchAnimation();

            _audioService?.PlaySfx(_switchItemSfxKey, transform.position);
        }

        private void ShowInventory()
        {
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

            _switchSequence.AppendCallback(() =>
            {
                _itemFinalView.SetActive(false);

                _itemAnimateShow.SetActive(false);
                _itemAnimateHide.SetActive(false);
                _itemAnimateShow.SetActive(true);
                _itemAnimateHide.SetActive(true);

                _showAnimator.Play("ShowItem", -1, 0f);
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
            if (_shakeTween != null && _shakeTween.IsActive())
                return;

            _itemFinalView.SetActive(false);
            _itemAnimateShow.SetActive(false);
            _itemAnimateHide.SetActive(false);
        }

        private void UpdateIcon(int index)
        {
            if (index >= 0 && index < _consumables.Length)
                _itemIconImage.sprite = _consumables[index].Icon;
        }

        private void RefreshCountText()
        {
            int index = _entity.CurrentItemIndex.Value;
            var chargesList = _entity.InventoryCharges;

            if (chargesList != null && index >= 0 && index < chargesList.Count)
            {
                int currentCharges = chargesList[index].Value;
                _countText.text = currentCharges.ToString();
            }
        }

        public override void Cleanup(Entity entity)
        {
            if (this == null)
                return;

            base.Cleanup(entity);

            foreach (var sub in _chargeSubscriptions)
                sub?.Dispose();

            _itemIntentDisposable?.Dispose();

            _fadeSequence?.Kill();
            _switchSequence?.Kill();
            _shakeTween?.Kill();
        }
    }
}