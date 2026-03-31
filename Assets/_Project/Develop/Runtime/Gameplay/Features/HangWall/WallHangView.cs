using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{
    public class WallHangView : EntityView
    {
        private static readonly int IsWallHangingKey = Animator.StringToHash("IsWallHanging");

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _viewContainer;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _sparksPS;
        [SerializeField] private ParticleSystem _debrisPS;
        [SerializeField] private float _effectOffset = 0.4f;

        [Header("Sword Vibration")]
        [SerializeField] private float _vibrationStrength = 0.03f;
        [SerializeField] private float _vibrationSpeed = 30f;

        [Header("Audio Settings")]
        [SerializeField] private string _hitPrefix = "WallHit";      // WallHit1, WallHit2...
        [SerializeField] private string _loopPrefix = "WallHitLoop"; // WallHitLoop1, WallHitLoop2...

        private AudioService _audioService;
        private IReadOnlyVariable<bool> _isWallHanging;
        private IReadOnlyVariable<float> _wallDirection;
        private IDisposable _isWallHangingDisposable;

        private string _activeLoopId;
        private Vector3 _defaultContainerPos;
        private float _vibrationTimer;
        private bool _isCurrentlyHanging;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;

            _isWallHanging = entity.IsWallHanging;
            _wallDirection = entity.WallDirection;

            if (_viewContainer != null)
                _defaultContainerPos = _viewContainer.localPosition;

            _isWallHangingDisposable = _isWallHanging.Subscribe(OnHangingStateChanged);

            ApplyState(_isWallHanging.Value);
        }

        private void Update()
        {
            HandleVibration();
        }

        private void OnHangingStateChanged(bool oldValue, bool newValue) => ApplyState(newValue);

        private void ApplyState(bool isHanging)
        {
            _isCurrentlyHanging = isHanging;
            _vibrationTimer = 0f;

            // 1. Анимация
            if (_animator != null)
                _animator.SetBool(IsWallHangingKey, isHanging);

            // 2. Эффекты
            ToggleEffects(isHanging);

            if (isHanging)
                PositionEffects();

            // 3. Звук
            HandleAudio(isHanging);

            // Сброс позиции контейнера при выходе
            if (!isHanging && _viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
        }

        private void HandleAudio(bool isHanging)
        {
            // Префикс из конфига "Hero Attack Hit"
            // Но судя по коду AudioService, мы передаем просто ID или часть префикса.
            // Если используем PlaySfxVariation, то добавим Hero Attack Hit к поиску, если это нужно сервису.

            if (isHanging)
            {
                // Одиночный удар при зацепе (вариации 1-4 согласно скрину)
                _audioService.PlaySfxVariation(_hitPrefix, 1, 4, UnityEngine.Random.Range(0.9f, 1.1f));

                // Зацикленный звук скольжения (вариации 1-2 согласно скрину)
                _activeLoopId = _audioService.PlaySfxVariationLoop(_loopPrefix, 1, 2);
            }
            else
            {
                if (!string.IsNullOrEmpty(_activeLoopId))
                {
                    _audioService.StopSfx(_activeLoopId);
                    _activeLoopId = null;
                }
            }
        }

        private void HandleVibration()
        {
            if (!_isCurrentlyHanging || _viewContainer == null) return;

            _vibrationTimer += Time.deltaTime * _vibrationSpeed;
            float offsetX = Mathf.Sin(_vibrationTimer) * _vibrationStrength * _wallDirection.Value;
            _viewContainer.localPosition = _defaultContainerPos + new Vector3(offsetX, 0f, 0f);
        }

        private void PositionEffects()
        {
            Vector3 offset = new Vector3(_wallDirection.Value * _effectOffset, 0f, 0f);

            if (_sparksPS != null) _sparksPS.transform.localPosition = offset;
            if (_debrisPS != null) _debrisPS.transform.localPosition = offset;
        }

        private void ToggleEffects(bool play)
        {
            if (play)
            {
                if (_sparksPS != null) _sparksPS.Play();
                if (_debrisPS != null) _debrisPS.Play();
            }
            else
            {
                if (_sparksPS != null) _sparksPS.Stop();
                if (_debrisPS != null) _debrisPS.Stop();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isWallHangingDisposable?.Dispose();

            if (!string.IsNullOrEmpty(_activeLoopId))
                _audioService.StopSfx(_activeLoopId);

            if (_viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
        }
    }
}