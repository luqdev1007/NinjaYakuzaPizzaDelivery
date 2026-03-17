using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RunTiltView : EntityView
    {
        [Header("Tilt")]
        [SerializeField, Range(0f, 30f)] private float _maxTiltAngle = 15f;
        [SerializeField, Min(0.1f)] private float _tiltSpeed = 10f;

        [Header("Squash & Stretch")]
        [SerializeField] private Vector3 _squashScale = new Vector3(1.2f, 0.8f, 1f);
        [SerializeField] private Vector3 _stretchScale = new Vector3(0.8f, 1.2f, 1f);
        [SerializeField, Min(0.1f)] private float _squashStretchSpeed = 12f;

        private Rigidbody2D _rigidbody;
        private float _maxSpeed;
        private float _previousVelocityX;
        private Transform _spriteTransform;
        private Vector3 _defaultScale;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _maxSpeed = entity.MoveSpeed.Value;
            _spriteTransform = transform;
            _defaultScale = transform.localScale;
        }

        private void Update()
        {
            if (_rigidbody == null) 
                return;

            UpdateTilt();
            UpdateSquashStretch();

            _previousVelocityX = _rigidbody.linearVelocity.x;
        }

        private void UpdateTilt()
        {
            float speedRatio = _rigidbody.linearVelocity.x / _maxSpeed;
            float targetAngle = -speedRatio * _maxTiltAngle;

            float currentZ = _spriteTransform.localEulerAngles.z;

            if (currentZ > 180f) 
                currentZ -= 360f;

            float newZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * _tiltSpeed);
            _spriteTransform.localEulerAngles = new Vector3(0f, 0f, newZ);
        }

        private void UpdateSquashStretch()
        {
            float acceleration = _rigidbody.linearVelocity.x - _previousVelocityX;
            float accelRatio = Mathf.Clamp01(Mathf.Abs(acceleration) / (Time.deltaTime * 50f));

            bool isGrounded = Mathf.Abs(_rigidbody.linearVelocity.y) < 0.1f;

            if (!isGrounded)
            {
                Vector3 targetScale = Vector3.Lerp(_defaultScale, _stretchScale, accelRatio);
                _spriteTransform.localScale = Vector3.Lerp(
                    _spriteTransform.localScale, targetScale, Time.deltaTime * _squashStretchSpeed);
            }
            else
            {
                Vector3 targetScale = Vector3.Lerp(_defaultScale, _squashScale, accelRatio);
                _spriteTransform.localScale = Vector3.Lerp(
                    _spriteTransform.localScale, targetScale, Time.deltaTime * _squashStretchSpeed);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _rigidbody = null;
        }
    }
}