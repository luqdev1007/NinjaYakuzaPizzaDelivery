using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class ChargedSlashAttackView : EntityView, IRequireAudioService
    {
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _chargingVfx;
        [SerializeField] private ParticleSystem _fireVfx;

        [Header("VFX Charge Settings")]
        [SerializeField] private float _minEmissionRate = 5f;
        [SerializeField] private float _maxEmissionRate = 100f;

        [Header("SFX Keys & Settings")]
        [SerializeField] private string _chargeStartSfxKey = "ChargeStart";
        [SerializeField] private string _chargeShootSfxKey = "ChargeShoot";

        [Tooltip("Задержка перед воспроизведением звука чарджа, чтобы отсечь микроклики")]
        [SerializeField] private float _chargeSfxDelay = 0.15f;

        private IReadOnlyVariable<float> _chargeSlashAttackCurrentTimer;
        private IReadOnlyVariable<float> _chargeSlashAttackRequiredTimer;

        private IAudioService _audioService;
        private bool _isCharging;
        private bool _hasPlayedChargeSfx;

        private IDisposable _isChargingDisposable;
        private IDisposable _spawnSlashDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _chargeSlashAttackCurrentTimer = entity.ChargeSlashAttackCurrentTimer;
            _chargeSlashAttackRequiredTimer = entity.ChargeSlashAttackRequiredTimer;

            _isChargingDisposable = entity.IsChargingSlashAttack.Subscribe(OnChargingStateChanged);
            _spawnSlashDisposable = entity.SpawnChargedSlashAtackEvent.Subscribe(OnAttackFired);
        }

        private void Update()
        {
            if (_isCharging && _chargeSlashAttackCurrentTimer != null && _chargeSlashAttackRequiredTimer != null)
            {
                float currentTimer = _chargeSlashAttackCurrentTimer.Value;
                float requiredTimer = _chargeSlashAttackRequiredTimer.Value;
                float progress = requiredTimer > 0f ? Mathf.Clamp01(currentTimer / requiredTimer) : 1f;

                UpdateChargeEmission(progress);

                if (!_hasPlayedChargeSfx && currentTimer >= _chargeSfxDelay)
                {
                    _audioService?.PlaySfx(_chargeStartSfxKey, transform.position);
                    _hasPlayedChargeSfx = true;
                }
            }
        }

        private void OnChargingStateChanged(bool old, bool current)
        {
            _isCharging = current;

            if (current)
            {
                _hasPlayedChargeSfx = false; 
                UpdateChargeEmission(0f);
                _chargingVfx.Play();
            }
            else
            {
                _chargingVfx.Stop();
            }
        }

        private void UpdateChargeEmission(float progress)
        {
            var emission = _chargingVfx.emission;
            emission.rateOverTime = Mathf.Lerp(_minEmissionRate, _maxEmissionRate, progress);
        }

        private void OnAttackFired()
        {
            _animator.SetTrigger(AttackTrigger);

            _fireVfx.Stop();
            _fireVfx.Play();

            _audioService?.PlaySfx(_chargeShootSfxKey, transform.position);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isChargingDisposable?.Dispose();
            _spawnSlashDisposable?.Dispose();

            _isCharging = false;
            _hasPlayedChargeSfx = false;
            _chargingVfx.Stop();
        }
    }
}