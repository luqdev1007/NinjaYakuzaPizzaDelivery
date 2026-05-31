using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class TargetingCoreSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private Entity _selfEntity;
        private Transform _transform;
        private ITargetSelector _targetSelector;

        private ReactiveVariable<bool> _intentTargeting;
        private ReactiveVariable<bool> _isTargetingActive;
        private ReactiveVariable<Entity> _currentTarget;

        private readonly float _holdThreshold = 0.4f;
        private readonly float _sqrMaxScanRadius = 144f;

        private float _holdTimer;
        private bool _wasTargetingIntendedLastFrame;
        private bool _holdActionTriggered;

        public TargetingCoreSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _selfEntity = entity;
            _transform = entity.Transform;

            _intentTargeting = entity.IntentSwitchTarget;
            _isTargetingActive = entity.IsTargetingActive;
            _currentTarget = entity.CurrentTarget;

            _targetSelector = new NearestDamagableTargetSelector(entity);
        }

        public void OnUpdate(float deltaTime)
        {
            bool currentIntent = _intentTargeting.Value;

            bool isPressedDown = currentIntent && !_wasTargetingIntendedLastFrame;
            bool isReleased = !currentIntent && _wasTargetingIntendedLastFrame;

            _wasTargetingIntendedLastFrame = currentIntent;

            if (isPressedDown)
            {
                _holdTimer = 0f;
                _holdActionTriggered = false;
            }

            if (currentIntent && !_holdActionTriggered)
            {
                _holdTimer += deltaTime;

                if (_holdTimer >= _holdThreshold)
                {
                    ToggleTargetingSystem();
                    _holdActionTriggered = true; 
                }
            }

            if (isReleased && !_holdActionTriggered)
            {
                ProcessTapAction();
            }

            if (_isTargetingActive.Value)
            {
                ValidateCurrentTarget();
            }
        }


        private void ProcessTapAction()
        {
            if (!_isTargetingActive.Value) 
                return;

            Entity nextTarget = FindNextClosestTarget(_currentTarget.Value);
            if (nextTarget != null)
            {
                _currentTarget.Value = nextTarget;
            }
        }

        private void ToggleTargetingSystem()
        {
            if (_isTargetingActive.Value)
            {
                // Ручное ВЫКЛЮЧЕНИЕ системы
                _isTargetingActive.Value = false;
                _currentTarget.Value = null;
            }
            else
            {
                // Ручное ВКЛЮЧЕНИЕ системы
                _isTargetingActive.Value = true;

                // Сразу пытаемся захватить кого-то в радиусе, если он есть
                Entity closest = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);
                if (closest != null && GetSqrDistanceTo(closest) <= _sqrMaxScanRadius)
                {
                    _currentTarget.Value = closest;
                }
                else
                {
                    // Если никого нет, цель остается null, но сама система РАБОТАЕТ и ждет врагов
                    _currentTarget.Value = null;
                }
            }
        }

        private void ValidateCurrentTarget()
        {
            Entity current = _currentTarget.Value;

            // Если текущей цели нет, она умерла или убежала слишком далеко
            if (current == null || !current.HasComponent<TakeDamageRequest>() || GetSqrDistanceTo(current) > _sqrMaxScanRadius)
            {
                // Локатор ищет новую цель
                Entity next = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);

                if (next != null && GetSqrDistanceTo(next) <= _sqrMaxScanRadius)
                {
                    _currentTarget.Value = next;
                }
                else
                {
                    // КРИТИЧЕСКОЕ ИЗМЕНЕНИЕ: Врагов нет. 
                    // Мы просто скидываем таргет, но НЕ выключаем _isTargetingActive!
                    _currentTarget.Value = null;
                }
            }
        }

        private Entity FindNextClosestTarget(Entity currentTarget)
        {
            Entity nextClosest = null;
            float minSqrDistance = float.MaxValue;

            foreach (Entity target in _entitiesLifeContext.Entities)
            {
                if (target == _selfEntity || target == currentTarget) 
                    continue;

                if (!target.HasComponent<TakeDamageRequest>()) 
                    continue;

                // if (EntitiesHelper.IsSameTeam(_selfEntity, target))
                // continue;

                float sqrDist = GetSqrDistanceTo(target);
                if (sqrDist < minSqrDistance && sqrDist <= _sqrMaxScanRadius)
                {
                    minSqrDistance = sqrDist;
                    nextClosest = target;
                }
            }

            return nextClosest ?? currentTarget;
        }

        private void ClearTargeting()
        {
            _currentTarget.Value = null;
            _isTargetingActive.Value = false;
        }

        private float GetSqrDistanceTo(Entity target)
        {
            if (target == null || target.Transform == null)
                return float.MaxValue;

            Vector2 offset = target.Transform.position - _transform.position;
            return offset.sqrMagnitude;
        }
    }
}