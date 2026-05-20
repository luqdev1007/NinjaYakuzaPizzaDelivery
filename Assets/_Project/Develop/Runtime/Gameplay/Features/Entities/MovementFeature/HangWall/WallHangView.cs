using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{
    public class WallHangView : EntityView, IRequireAudioService
    {
        private static readonly int IsWallHangingKey = Animator.StringToHash("IsWallHanging");

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _viewContainer;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _sparksPS;
        [SerializeField] private ParticleSystem _debrisPS;

        [Tooltip("Сдвиг эффектов вперед по локальной оси X")]
        [SerializeField] private float _effectOffset = 0.4f;

        [Header("Sword Vibration")]
        [SerializeField] private float _vibrationStrength = 0.03f;
        [SerializeField] private float _vibrationSpeed = 30f;

        [Header("SFX Keys")]
        [SerializeField] private string _hitSfxKey = "WallHit";
        [SerializeField] private string _loopSfxKey = "WallHitLoop";

        private IAudioService _audioService;
        private IReadOnlyVariable<bool> _isWallHanging;
        private IDisposable _isWallHangingDisposable;

        private Vector3 _defaultContainerPos;
        private float _vibrationTimer;
        private bool _isCurrentlyHanging;
        private string _activeLoopKey;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isWallHanging = entity.IsWallHanging;
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

            _animator.SetBool(IsWallHangingKey, isHanging);

            ToggleEffects(isHanging);

            if (isHanging)
                PositionEffects();

            HandleAudio(isHanging);

            if (!isHanging)
                _viewContainer.localPosition = _defaultContainerPos;
        }

        private void HandleAudio(bool isHanging)
        {
            if (isHanging)
            {
                // Передаем ключ напрямую — SoundData сам выберет случайный клип из инспектора
                _audioService.PlaySfx(_hitSfxKey, transform.position);

                _activeLoopKey = _loopSfxKey;
                _audioService.PlaySfxLoop(_activeLoopKey, transform.position);
            }
            else
            {
                if (!string.IsNullOrEmpty(_activeLoopKey))
                {
                    _audioService.StopSfx(_activeLoopKey);
                    _activeLoopKey = null;
                }
            }
        }

        private void HandleVibration()
        {
            if (!_isCurrentlyHanging)
                return;

            _vibrationTimer += Time.deltaTime * _vibrationSpeed;

            float offsetX = Mathf.Sin(_vibrationTimer) * _vibrationStrength;
            _viewContainer.localPosition = _defaultContainerPos + new Vector3(offsetX, 0f, 0f);
        }

        private void PositionEffects()
        {
            Vector3 offset = new Vector3(_effectOffset, 0f, 0f);

            _sparksPS.transform.localPosition = offset;
            _debrisPS.transform.localPosition = offset;
        }

        private void ToggleEffects(bool play)
        {
            if (play)
            {
                _sparksPS.Play();
                _debrisPS.Play();
            }
            else
            {
                _sparksPS.Stop();
                _debrisPS.Stop();
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isWallHangingDisposable?.Dispose();

            if (!string.IsNullOrEmpty(_activeLoopKey))
            {
                _audioService.StopSfx(_activeLoopKey);
                _activeLoopKey = null;
            }

            _viewContainer.localPosition = _defaultContainerPos;
        }
    }
}