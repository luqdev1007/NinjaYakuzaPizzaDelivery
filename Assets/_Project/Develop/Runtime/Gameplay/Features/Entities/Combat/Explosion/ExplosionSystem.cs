using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Explosion;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Explosion
{
    /// <summary>
    /// Площадной взрыв призрака-камикадзе. Исполняет DetonationRequest: собирает
    /// всех в радиусе, раздаёт импульс и (только при Natural) урон, после чего
    /// самоуничтожается.
    /// </summary>
    /// <remarks>
    /// Тикового канала нет — работа в колбэке запроса, по образцу BounceSystem и
    /// PlungeDamageOnImpactSystem.
    ///
    /// Урон идёт через EntitiesHelper.TryTakeDamageIgnoringTeams: взрыв обязан
    /// задевать всех в радиусе, включая других врагов (та же команда Enemies) и
    /// пропы (команды нет вовсе). Тим-фильтр здесь выключил бы половину целей.
    /// </remarks>
    public class ExplosionSystem : IInitializableSystem, IDisposableSystem
    {
        // Направление отброса всегда уводится вверх: к нормализованному вектору от
        // эпицентра добавляется единица по Y. Взрыв, толкающий строго вбок, читается
        // как удар, а не как подброс, и не даёт игроку вертикали для продолжения
        // связки. Плюс это страхует вырожденный случай, когда цель стоит ровно на
        // одной высоте с призраком.
        private const float UpwardKnockbackBias = 1f;

        private readonly CollidersRegistryService _collidersRegistryService;

        // Переиспользуемый между взрывами буфер уникальных целей. У одной сущности
        // бывает несколько зарегистрированных коллайдеров — у призрака это корневая
        // капсула и дочерний Jumpable, — и без дедупликации она получила бы урон и
        // импульс дважды.
        private readonly HashSet<Entity> _processedTargets = new();

        private Entity _selfEntity;
        private Transform _selfTransform;

        private ReactiveVariable<float> _explosionRadius;
        private ReactiveVariable<float> _explosionDamage;
        private ReactiveVariable<float> _explosionKnockbackForce;
        private ReactiveVariable<float> _forcedKnockbackMultiplier;
        private ReactiveVariable<bool> _hasDetonated;
        private ReactiveVariable<float> _currentHealth;
        private ReactiveEvent<DetonationKind> _detonationEvent;

        private LayerMask _explosionLayerMask;

        private IDisposable _requestDisposable;

        public ExplosionSystem(CollidersRegistryService collidersRegistryService)
        {
            _collidersRegistryService = collidersRegistryService;
        }

        public void OnInit(Entity entity)
        {
            _selfEntity = entity;
            _selfTransform = entity.Transform;

            _explosionRadius = entity.ExplosionRadius;
            _explosionDamage = entity.ExplosionDamage;
            _explosionKnockbackForce = entity.ExplosionKnockbackForce;
            _forcedKnockbackMultiplier = entity.ForcedExplosionKnockbackMultiplier;
            _hasDetonated = entity.HasDetonated;
            _currentHealth = entity.CurrentHealth;
            _detonationEvent = entity.DetonationEvent;

            _explosionLayerMask = entity.ExplosionLayerMask;

            _requestDisposable = entity.DetonationRequest.Subscribe(OnDetonationRequest);
        }

        public void OnDispose()
        {
            _requestDisposable?.Dispose();
        }

        private void OnDetonationRequest(DetonationKind detonationKind)
        {
            if (_hasDetonated.Value)
            {
                return;
            }

            // Защёлка ставится ДО самоуничтожения. Ниже CurrentHealth = 0 запускает
            // цепочку DeathSystem -> IsDead -> ForcedDetonationSystem, и та обязана
            // увидеть уже взведённый флаг, иначе взрыв случится второй раз.
            _hasDetonated.Value = true;

            ApplyExplosionToTargets(detonationKind);

            _detonationEvent.Invoke(detonationKind);

            // Самоуничтожение мимо TakeDamageRequest: тот прошёл бы через
            // canApplyDamage, где висит DamageCooldownTimer, и запрос мог бы
            // отфильтроваться, оставив взорвавшегося призрака живым.
            // DeathSystem подхватит mustDie на ближайшем Update-тике.
            _currentHealth.Value = 0;
        }

        private void ApplyExplosionToTargets(DetonationKind detonationKind)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                _selfTransform.position,
                _explosionRadius.Value,
                _explosionLayerMask.value);

            _processedTargets.Clear();

            foreach (Collider2D collider in hitColliders)
            {
                Entity target = _collidersRegistryService.GetBy(collider);

                if (target == null)
                {
                    continue;
                }

                // Собственная капсула лежит на слое Enemies и попадает в выборку.
                // Без этой проверки призрак нанёс бы урон сам себе.
                if (target == _selfEntity)
                {
                    continue;
                }

                if (_processedTargets.Add(target) == false)
                {
                    continue;
                }

                // Сущность могла быть уничтожена в этом же кадре — Transform уже
                // мёртв, хотя ссылка на Entity ещё живая (образец:
                // NearestDamagableTargetSelector.GetSqrDistanceTo, RotateToTargetState).
                if (target.Transform == null)
                {
                    continue;
                }

                ApplyExplosionTo(target, detonationKind);
            }

            _processedTargets.Clear();
        }

        private void ApplyExplosionTo(Entity target, DetonationKind detonationKind)
        {
            Vector2 knockbackForce = CalculateKnockbackFor(target, detonationKind);

            // Импульс раздаётся ВСЕГДА, независимо от вида детонации: вынужденный
            // взрыв тоже толкает, просто слабее и без урона.
            if (target.TryGetExplosionImpulseRequest(out ReactiveEvent<Vector2> impulseRequest))
            {
                impulseRequest.Invoke(knockbackForce);
            }

            if (detonationKind != DetonationKind.Natural)
            {
                return;
            }

            DamageData damageData = new DamageData
            {
                Amount = _explosionDamage.Value,
                SourcePosition = _selfTransform.position,
                KnockbackForce = knockbackForce,
                Type = DamageType.Blunt
            };

            EntitiesHelper.TryTakeDamageIgnoringTeams(_selfEntity, target, damageData);
        }

        private Vector2 CalculateKnockbackFor(Entity target, DetonationKind detonationKind)
        {
            Vector2 offset = target.Transform.position - _selfTransform.position;
            Vector2 direction = offset.normalized;

            // Позиции совпали — нормализация дала ноль, направления нет. Толкаем вверх.
            if (direction.sqrMagnitude <= 0f)
            {
                direction = Vector2.up;
            }

            Vector2 arcDirection = new Vector2(direction.x, Mathf.Abs(direction.y) + UpwardKnockbackBias).normalized;

            float magnitude = _explosionKnockbackForce.Value;

            if (detonationKind == DetonationKind.Forced)
            {
                magnitude *= _forcedKnockbackMultiplier.Value;
            }

            return arcDirection * magnitude;
        }
    }
}
