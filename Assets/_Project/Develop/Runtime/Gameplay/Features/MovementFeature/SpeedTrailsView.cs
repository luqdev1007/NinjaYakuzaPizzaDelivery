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
        [SerializeField] private float _offDelay = 0.3f;
        [SerializeField] private float _trailOffset = 0.5f;
        [SerializeField] private float _rotationSpeed = 20f;

        private Rigidbody2D _rigidbody;
        private Vector3 _lastTrailPos;
        private bool _isEmitting;
        private float _offTimer;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            SetTrailsActive(false);
        }

        private void Update()
        {
            if (_rigidbody == null)
                return;

            Vector2 velocity = _rigidbody.linearVelocity;
            float speed = velocity.magnitude;

            if (speed >= _speedOnThreshold)
            {
                _offTimer = _offDelay;

                if (!_isEmitting)
                    SetTrailsActive(true);

                UpdateTrailPosition(velocity);
            }
            else if (_isEmitting && speed < _speedOffThreshold)
            {
                _offTimer -= Time.deltaTime;

                if (_offTimer <= 0f)
                    SetTrailsActive(false);
            }
        }

        private void UpdateTrailPosition(Vector2 velocity)
        {
            Vector2 opposite = -velocity.normalized * _trailOffset;
            Vector3 targetLocalPos = new Vector3(opposite.x, opposite.y, 0f);

            foreach (TrailRenderer trail in _trails)
            {
                Vector3 newPos = Vector3.Lerp(
                    trail.transform.localPosition,
                    targetLocalPos,
                    Time.deltaTime * _rotationSpeed);

                if (Vector3.Distance(newPos, _lastTrailPos) > 0.05f)
                    trail.Clear();

                trail.transform.localPosition = newPos;
            }

            _lastTrailPos = _trails[0].transform.localPosition;
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
