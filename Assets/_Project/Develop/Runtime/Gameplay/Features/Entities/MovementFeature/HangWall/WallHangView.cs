using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
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

        [Tooltip("Сдвиг эффектов вперед по локальной оси X")]
        [SerializeField] private float _effectOffset = 0.4f;

        [Header("Sword Vibration")]
        [SerializeField] private float _vibrationStrength = 0.03f;
        [SerializeField] private float _vibrationSpeed = 30f;

        private IReadOnlyVariable<bool> _isWallHanging;
        private IDisposable _isWallHangingDisposable;

        private Vector3 _defaultContainerPos;
        private float _vibrationTimer;
        private bool _isCurrentlyHanging;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isWallHanging = entity.IsWallHanging;

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

            _animator.SetBool(IsWallHangingKey, isHanging);

            ToggleEffects(isHanging);

            if (isHanging)
                PositionEffects();

            if (!isHanging && _viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
        }

        private void HandleVibration()
        {
            if (!_isCurrentlyHanging || _viewContainer == null)
                return;

            _vibrationTimer += Time.deltaTime * _vibrationSpeed;

            // Т.к. родитель крутится по оси Y, локальный X всегда "вперед" к стене
            float offsetX = Mathf.Sin(_vibrationTimer) * _vibrationStrength;
            _viewContainer.localPosition = _defaultContainerPos + new Vector3(offsetX, 0f, 0f);
        }

        private void PositionEffects()
        {
            // Сдвиг всегда по локальному X (вперед к стене)
            Vector3 offset = new Vector3(_effectOffset, 0f, 0f);

            if (_sparksPS != null)
                _sparksPS.transform.localPosition = offset;

            if (_debrisPS != null)
                _debrisPS.transform.localPosition = offset;
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

            if (_viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
        }
    }
}