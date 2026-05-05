using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class HeroMovementVfxView : EntityView
    {
        [Header("Continuous VFX (In Hierarchy)")]
        [SerializeField] private ParticleSystem _runDustPS;
        [SerializeField] private TrailRenderer[] _trails;

        [Header("Instant VFX (From Pool)")]
        [SerializeField] private ParticleSystem _brakeDustPrefab;
        [SerializeField] private ParticleSystem _startDustPrefab;
        [SerializeField] private Transform _groundPoint;

        [Header("Settings")]
        [SerializeField] private float _runThreshold = 2f;
        [SerializeField] private float _brakeThreshold = 4f;
        [SerializeField] private float _brakeCooldown = 0.25f;
        [SerializeField] private float _trailSpeedOn = 8f;
        [SerializeField] private float _trailSpeedOff = 5f;

        private IVfxPoolService _vfxPool;
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<bool> _isGrounded;
        private IReadOnlyVariable<bool> _isMoving;

        private float _prevVelocityX;
        private bool _wasMoving;
        private float _brakeTimer;

        protected override void OnDependencyResolve(DIContainer container)
        {
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGrounded = entity.IsGrounded;
            _isMoving = entity.IsMoving;

            if (_runDustPS != null)
            {
                var emission = _runDustPS.emission;
                emission.enabled = false;
            }

            SetTrails(false);
        }

        private void Update()
        {
            if (_rigidbody == null) return;

            float vX = _rigidbody.linearVelocity.x;
            float absVX = Mathf.Abs(vX);
            bool grounded = _isGrounded.Value;
            bool moving = _isMoving.Value;

            if (_brakeTimer > 0) _brakeTimer -= Time.deltaTime;

            HandleRunDust(grounded, moving, absVX);
            HandleBrakeDust(grounded, vX, absVX);
            HandleStartDust(grounded, moving, absVX);
            HandleTrails(absVX);

            _prevVelocityX = vX;
            _wasMoving = moving;
        }

        private void HandleRunDust(bool grounded, bool moving, float absVX)
        {
            if (_runDustPS == null) return;

            bool shouldEmit = grounded && moving && absVX > _runThreshold;
            var emission = _runDustPS.emission;

            if (emission.enabled != shouldEmit)
            {
                emission.enabled = shouldEmit;

                if (shouldEmit && !_runDustPS.isPlaying)
                    _runDustPS.Play();
            }
        }

        private void HandleBrakeDust(bool grounded, float vX, float absVX)
        {
            if (!grounded || _brakeDustPrefab == null || _vfxPool == null || _brakeTimer > 0) return;

            bool isBraking = (Mathf.Abs(_prevVelocityX) > _brakeThreshold) &&
                             (Mathf.Sign(vX) != Mathf.Sign(_prevVelocityX) || absVX < 0.5f);

            if (isBraking)
            {
                _vfxPool.Spawn(_brakeDustPrefab, _groundPoint.position, Quaternion.identity);
                _brakeTimer = _brakeCooldown; 
            }
        }

        private void HandleStartDust(bool grounded, bool moving, float absVX)
        {
            if (grounded && moving && !_wasMoving && absVX > 1f)
            {
                if (_startDustPrefab != null)
                    _vfxPool.Spawn(_startDustPrefab, _groundPoint.position, Quaternion.identity);
            }
        }

        private void HandleTrails(float speed)
        {
            if (speed >= _trailSpeedOn) SetTrails(true);
            else if (speed < _trailSpeedOff) SetTrails(false);
        }

        private void SetTrails(bool active)
        {
            if (_trails == null) return;
            foreach (var t in _trails)
            {
                if (t != null) t.emitting = active;
            }
        }
    }
}