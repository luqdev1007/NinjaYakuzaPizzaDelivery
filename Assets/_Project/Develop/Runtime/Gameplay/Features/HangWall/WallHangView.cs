using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Infrastructure.DI;
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
        [SerializeField] private string _hitPrefix = "WallHit";
        [SerializeField] private string _loopPrefix = "WallHitLoop";

        private AudioService _audioService;
        private IReadOnlyVariable<float> _wallDirection;
        private IDisposable _isWallHangingDisposable;

        private string _activeLoopId;
        private Vector3 _defaultContainerPos;
        private float _vibrationTimer;
        private bool _isCurrentlyHanging;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnDependencyResolve(DIContainer container)
        {
            _audioService = container.Resolve<AudioService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _wallDirection = entity.WallDirection;

            if (_viewContainer != null)
                _defaultContainerPos = _viewContainer.localPosition;

            _isWallHangingDisposable = entity.IsWallHanging.Subscribe(OnHangingStateChanged);

            ApplyState(entity.IsWallHanging.Value);
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

            if (_animator != null)
                _animator.SetBool(IsWallHangingKey, isHanging);

            ToggleEffects(isHanging);

            if (isHanging)
                PositionEffects();

            HandleAudio(isHanging);

            if (!isHanging && _viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
        }

        private void HandleAudio(bool isHanging)
        {
            if (_audioService == null) return;

            if (isHanging)
            {
                _audioService.PlaySfxVariation(_hitPrefix, 1, 4, UnityEngine.Random.Range(0.9f, 1.1f));
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
            if (!_isCurrentlyHanging || _viewContainer == null || _wallDirection == null) return;

            _vibrationTimer += Time.deltaTime * _vibrationSpeed;
            float offsetX = Mathf.Sin(_vibrationTimer) * _vibrationStrength * _wallDirection.Value;
            _viewContainer.localPosition = _defaultContainerPos + new Vector3(offsetX, 0f, 0f);
        }

        private void PositionEffects()
        {
            if (_wallDirection == null) return;

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

            if (!string.IsNullOrEmpty(_activeLoopId) && _audioService != null)
            {
                _audioService.StopSfx(_activeLoopId);
                _activeLoopId = null;
            }

            if (_viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
        }
    }
}