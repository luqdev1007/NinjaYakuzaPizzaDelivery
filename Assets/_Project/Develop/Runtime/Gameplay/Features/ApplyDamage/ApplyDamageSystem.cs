using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem, IUpdatableSystem
    {
        private readonly string _entityId;
        private readonly AudioService _audioService;

        private ReactiveEvent<DamageData> _damageRequest;
        private ReactiveEvent<DamageData> _damageEvent;
        private ReactiveVariable<float> _cooldownTimer;
        private float _defaultCooldown;
        private ReactiveVariable<float> _health;
        private ICompositeCondition _canApplyDamage;
        private IDisposable _requestDisposable;

        public ApplyDamageSystem(string entityId, AudioService audioService)
        {
            _entityId = entityId;
            _audioService = audioService;
        }

        public void OnInit(Entity entity)
        {
            _damageRequest = entity.TakeDamageRequest;
            _damageEvent = entity.TakeDamageEvent;
            _health = entity.CurrentHealth;
            _canApplyDamage = entity.CanApplyDamage;
            _cooldownTimer = entity.DamageCooldownTimer;
            _defaultCooldown = entity.DamageCooldown.Value;

            _requestDisposable = _damageRequest.Subscribe(OnDamageRequest);
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cooldownTimer.Value > 0)
                _cooldownTimer.Value -= deltaTime;
        }

        private void OnDamageRequest(DamageData damage)
        {
            if (_canApplyDamage.Evaluate() == false)
                return;

            _health.Value = MathF.Max(_health.Value - damage.Amount, 0);
            _cooldownTimer.Value = _defaultCooldown;

            _audioService.PlaySfxByPrefix(_entityId + "Hit", true);

            _damageEvent.Invoke(damage);
        }

        public void OnDispose() => _requestDisposable.Dispose();
    }
}