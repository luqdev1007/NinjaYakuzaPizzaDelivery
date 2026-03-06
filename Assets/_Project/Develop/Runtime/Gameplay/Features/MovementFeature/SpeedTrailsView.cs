using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class SpeedTrailsView : EntityView
    {
        [SerializeField] private TrailRenderer[] _trails;
        [SerializeField] private float _speedOnThreshold = 8f;
        [SerializeField] private float _speedOffThreshold = 5f;
        [SerializeField] private float _offDelay = 0.3f; // секунд до выключения

        private Rigidbody2D _rigidbody;
        private bool _isEmitting;
        private float _offTimer; // таймер обратного отсчёта

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            SetTrailsActive(false);
        }

        private void Update()
        {
            if (_rigidbody == null) return;

            float speed = _rigidbody.linearVelocity.magnitude;

            if (speed >= _speedOnThreshold)
            {
                // включаем сразу и сбрасываем таймер
                _offTimer = _offDelay;
                if (!_isEmitting)
                    SetTrailsActive(true);
            }
            else if (_isEmitting && speed < _speedOffThreshold)
            {
                // считаем таймер вниз перед выключением
                _offTimer -= Time.deltaTime;
                if (_offTimer <= 0f)
                    SetTrailsActive(false);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _rigidbody = null;
        }

        private void SetTrailsActive(bool active)
        {
            _isEmitting = active;
            foreach (TrailRenderer trail in _trails)
                trail.emitting = active;
        }
    }
}