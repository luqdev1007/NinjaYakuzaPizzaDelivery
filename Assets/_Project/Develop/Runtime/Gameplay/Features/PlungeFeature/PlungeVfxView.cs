using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Infrastructure.DI; 
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeVfxView : EntityView
    {
        [Header("Animator")]
        [SerializeField] private Animator _animator;
        private static readonly int IsPlungingKey = Animator.StringToHash("IsPlunging");

        [Header("Flight VFX (In Hierarchy)")]
        [SerializeField] private ParticleSystem _airConePS;
        [SerializeField] private ParticleSystem[] _fireCones;
        [SerializeField] private float _fullPowerTime = 0.5f;
        [SerializeField] private float _maxAirEmission = 40f;
        [SerializeField] private float _maxFireEmission = 30f;

        [Header("Impact VFX (From Pool)")]
        [SerializeField] private ParticleSystem _impactPrefab;

        private IVfxPoolService _vfxPool;
        private IReadOnlyVariable<bool> _isPlunging;
        private IReadOnlyVariable<bool> _isGrounded;
        private float _flightTimer;
        private IDisposable _plungeSub;
        private IDisposable _groundedSub;

        protected override void OnDependencyResolve(DIContainer container)
        {
            _vfxPool = container.Resolve<IVfxPoolService>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isPlunging = entity.IsPlunging;
            _isGrounded = entity.IsGrounded;

            _plungeSub = _isPlunging.Subscribe((_, isPlunging) => {
                _animator.SetBool(IsPlungingKey, isPlunging);
                if (isPlunging) StartFlight(); else StopFlight();
            });

            _groundedSub = _isGrounded.Subscribe((_, grounded) => {
                if (grounded && _flightTimer > 0.1f) PlayImpact();
            });
        }

        private void Update()
        {
            if (!_isPlunging.Value) return;

            _flightTimer += Time.deltaTime;
            float ratio = Mathf.Clamp01(_flightTimer / _fullPowerTime);
            UpdateVfx(ratio);
        }

        private void UpdateVfx(float ratio)
        {
            if (_airConePS != null)
            {
                var emission = _airConePS.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxAirEmission, ratio);
            }

            float fireRatio = Mathf.InverseLerp(0.4f, 1.0f, ratio);
            foreach (var ps in _fireCones)
            {
                if (ps == null) continue;
                var emission = ps.emission;
                emission.rateOverTime = Mathf.Lerp(0, _maxFireEmission, fireRatio);

                if (fireRatio > 0.05f && !ps.isPlaying) ps.Play();
            }
        }

        private void StartFlight()
        {
            _flightTimer = 0f;
            _airConePS?.Play();
        }

        private void PlayImpact()
        {
            if (_impactPrefab != null && _vfxPool != null)
            {
                float impactScale = Mathf.Clamp(_flightTimer / _fullPowerTime, 0.5f, 1.5f);

                var vfx = _vfxPool.Spawn(_impactPrefab, transform.position, Quaternion.identity);

                var main = vfx.main;
                main.startSizeMultiplier = impactScale;
            }

            StopFlight();
        }

        private void StopFlight()
        {
            _airConePS?.Stop();
            foreach (var ps in _fireCones) ps?.Stop();
            _flightTimer = 0f;
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _plungeSub?.Dispose();
            _groundedSub?.Dispose();
        }
    }
}