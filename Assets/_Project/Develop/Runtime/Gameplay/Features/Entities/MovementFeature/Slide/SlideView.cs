using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Slide
{
    public class SlideView : EntityView, IRequireAudioService
    {
        private static readonly int IsSlidingKey = Animator.StringToHash("IsSliding");

        [Header("Animator")]
        [SerializeField] private Animator _animator;

        [Header("VFX - Friction & Dust")]
        [SerializeField] private ParticleSystem _frictionPS;
        [SerializeField] private float _maxEmissionRate = 50f;
        [SerializeField] private float _vfxRampUpTime = 0.3f;

        [Header("Visual Deformation")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _slideStretchX = 1.3f;
        [SerializeField] private float _slideSquashY = 0.7f;
        [SerializeField] private float _slideTiltAngle = 12f;
        [SerializeField] private float _lerpSpeed = 10f;
        [SerializeField] private float _slopeRotationLerp = 12f;

        [Header("SFX Keys")]
        [SerializeField] private string _slideKey = "Slide";
        [SerializeField] private string _slideLoopKey = "SlideLoop";

        private IAudioService _audioService;
        private Entity _entity;
        private Transform _transform;
        private IReadOnlyVariable<bool> _isSliding;
        private IReadOnlyVariable<bool> _isOnSlope;
        private IReadOnlyVariable<MovementStates> _movementState;

        private float _slideTimer;
        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;

        private IDisposable _slideDisposable;
        private IDisposable _stateDisposable;
        private bool _isInitialized;
        private bool _wasSlidingLastFrame;
        private string _activeLoopKey;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;
            _transform = entity.Transform;
            _isSliding = entity.IsSliding;
            _isOnSlope = entity.IsOnSlope;
            _movementState = entity.CurrentMovementState;

            _defaultScale = _viewContainer != null ? _viewContainer.localScale : Vector3.one;
            _defaultRotation = _viewContainer != null ? _viewContainer.localRotation : Quaternion.identity;

            _slideDisposable = _isSliding.Subscribe((old, cur) => EvaluateSlidingState());
            _stateDisposable = _movementState.Subscribe((old, cur) => EvaluateSlidingState());

            _wasSlidingLastFrame = IsCurrentlySliding();
            _animator.SetBool(IsSlidingKey, _wasSlidingLastFrame);

            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
                return;

            HandleRotationAndDeformation();

            if (!IsCurrentlySliding())
                return;

            _slideTimer += Time.deltaTime;
            float intensity = Mathf.Clamp01(_slideTimer / _vfxRampUpTime);

            UpdateVFX(intensity);
        }

        private bool IsCurrentlySliding()
        {
            return _isSliding.Value || _movementState.Value == MovementStates.Sliding;
        }

        private void EvaluateSlidingState()
        {
            bool isSlidingNow = IsCurrentlySliding();

            if (isSlidingNow == _wasSlidingLastFrame)
                return;

            _wasSlidingLastFrame = isSlidingNow;
            _animator.SetBool(IsSlidingKey, isSlidingNow);

            if (isSlidingNow)
            {
                _slideTimer = 0f;
                if (_frictionPS != null) _frictionPS.Play();

                // Выбираем аудио-ключ в зависимости от того, на склоне мы или на ровной поверхности
                _activeLoopKey = _isOnSlope.Value ? _slideLoopKey : _slideKey;
                _audioService?.PlaySfxLoop(_activeLoopKey, transform.position);
            }
            else
            {
                StopEffects();
            }
        }

        private void HandleRotationAndDeformation()
        {
            if (_viewContainer == null)
                return;

            bool isSliding = IsCurrentlySliding();

            // --- Деформация масштаба ---
            Vector3 targetScale = isSliding
                ? new Vector3(_defaultScale.x * _slideStretchX, _defaultScale.y * _slideSquashY, _defaultScale.z)
                : _defaultScale;

            _viewContainer.localScale = Vector3.Lerp(_viewContainer.localScale, targetScale, Time.deltaTime * _lerpSpeed);

            // --- Вращение ---
            if (isSliding)
            {
                float targetZ = 0f;

                if (_isOnSlope.Value)
                {
                    Vector3 worldNormal = _entity.SlopeNormal.Value;
                    Vector3 localNormal = _transform.InverseTransformDirection(worldNormal);
                    targetZ = Mathf.Atan2(localNormal.x, localNormal.y) * -Mathf.Rad2Deg;
                }
                else
                {
                    float direction = Mathf.Sign(_entity.LookDirectionX.Value);
                    targetZ = -_slideTiltAngle * direction;

                    if (_transform.localEulerAngles.y > 90f || _transform.localScale.x < 0f)
                    {
                        targetZ = -targetZ;
                    }
                }

                Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);
                _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, targetRot, Time.deltaTime * _slopeRotationLerp);
            }
            else
            {
                if (!_isOnSlope.Value)
                {
                    _viewContainer.localRotation = Quaternion.Lerp(_viewContainer.localRotation, Quaternion.identity, Time.deltaTime * _lerpSpeed);
                }
            }
        }

        private void UpdateVFX(float intensity)
        {
            if (_frictionPS == null) return;

            var emission = _frictionPS.emission;
            emission.rateOverTime = Mathf.Lerp(5f, _maxEmissionRate, intensity);
        }

        private void StopEffects()
        {
            if (_frictionPS != null) _frictionPS.Stop();

            if (!string.IsNullOrEmpty(_activeLoopKey))
            {
                _audioService?.StopSfx(_activeLoopKey);
                _activeLoopKey = null;
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _isInitialized = false;

            StopEffects();
            _slideDisposable?.Dispose();
            _stateDisposable?.Dispose();

            if (_viewContainer != null)
            {
                _viewContainer.localScale = _defaultScale;
                _viewContainer.localRotation = _defaultRotation;
            }
        }
    }
}