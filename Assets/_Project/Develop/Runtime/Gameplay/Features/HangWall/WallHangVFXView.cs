using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.HangWall
{
    public class WallHangVFXView : EntityView
    {
        [Header("Sparks")]
        [SerializeField] private ParticleSystem _sparksPS;

        [Header("Wall Debris")]
        [SerializeField] private ParticleSystem _debrisPS;

        [Header("Sword Vibration")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _vibrationStrength = 0.03f;
        [SerializeField] private float _vibrationSpeed = 30f;

        private IReadOnlyVariable<bool> _isWallHanging;
        private IReadOnlyVariable<float> _wallDirection;
        private IDisposable _isWallHangingDisposable;

        private bool _hanging;
        private float _vibrationTimer;
        private Vector3 _defaultContainerPos;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isWallHanging = entity.IsWallHanging;
            _wallDirection = entity.WallDirection;
            _isWallHangingDisposable = _isWallHanging.Subscribe(OnIsWallHangingChanged);

            if (_viewContainer != null)
                _defaultContainerPos = _viewContainer.localPosition;
        }

        private void Update()
        {
            if (!_hanging)
            {
                if (_viewContainer != null)
                {
                    _viewContainer.localPosition = Vector3.Lerp(
                        _viewContainer.localPosition,
                        _defaultContainerPos,
                        Time.deltaTime * 10f);
                }
                return;
            }

            _vibrationTimer += Time.deltaTime * _vibrationSpeed;

            if (_viewContainer != null)
            {
                float offsetX = Mathf.Sin(_vibrationTimer) * _vibrationStrength * _wallDirection.Value;
                _viewContainer.localPosition = _defaultContainerPos + new Vector3(offsetX, 0f, 0f);
            }

            PositionEffects();
        }

        private void PositionEffects()
        {
            if (_sparksPS == null)
                return;

            // позиционируем искры на стороне стены
            Vector3 offset = new Vector3(_wallDirection.Value * 0.4f, 0f, 0f);
            _sparksPS.transform.localPosition = offset;

            if (_debrisPS != null)
                _debrisPS.transform.localPosition = offset;
        }

        private void OnIsWallHangingChanged(bool oldValue, bool value)
        {
            _hanging = value;
            _vibrationTimer = 0f;

            if (_sparksPS != null)
            {
                if (value) _sparksPS.Play();
                else _sparksPS.Stop();
            }

            if (_debrisPS != null)
            {
                if (value) _debrisPS.Play();
                else _debrisPS.Stop();
            }

            if (!value && _viewContainer != null)
                _viewContainer.localPosition = _defaultContainerPos;
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