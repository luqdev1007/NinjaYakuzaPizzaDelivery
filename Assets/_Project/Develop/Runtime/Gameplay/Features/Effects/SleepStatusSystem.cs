using Assets._Project.Develop.Runtime.Gameplay.Common;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Effects
{
    public class SleepStatusSystem : IUpdatableSystem
    {
        private readonly List<Entity> _sleepingEntities = new();
        private readonly Dictionary<Entity, float> _timers = new();

        public void OnUpdate(float deltaTime)
        {
            // Здесь должна быть логика получения всех сущностей из твоего мира/пула
            // Но для простоты, предположим, что мы мониторим активные

            for (int i = _sleepingEntities.Count - 1; i >= 0; i--)
            {
                var entity = _sleepingEntities[i];

                if (entity.IsAsleep.Value)
                {
                    _timers[entity] -= deltaTime;

                    if (_timers[entity] <= 0)
                    {
                        entity.IsAsleep.Value = false;
                        _timers.Remove(entity);
                        _sleepingEntities.RemoveAt(i);
                        Debug.Log($"Entity {entity.Transform.gameObject.name} проснулся!");
                    }
                }
            }
        }

        // Метод для регистрации сна (можно вызывать из Projectile или через Reactive-подписку)
        public void ApplySleep(Entity entity, float duration)
        {
            if (!entity.HasComponent<IsAsleep>()) entity.AddIsAsleep();

            entity.IsAsleep.Value = true;
            _timers[entity] = duration;

            if (!_sleepingEntities.Contains(entity))
                _sleepingEntities.Add(entity);
        }
    }
}
