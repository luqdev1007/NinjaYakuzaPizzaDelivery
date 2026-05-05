using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootDistanceCollectSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
    {
        private readonly EntitiesLifeContext _lifeContext;
        private Entity _hero;

        private float _sqrCollectDistance;
        private IDisposable _rangeSubscription;

        public LootDistanceCollectSystem(EntitiesLifeContext lifeContext)
        {
            _lifeContext = lifeContext;
        }

        public void OnInit(Entity entity)
        {
            _hero = entity;

            _rangeSubscription = _hero.CollectRange.Subscribe((oldValue, newValue) => UpdateSqrDistance(newValue));

            UpdateSqrDistance(_hero.CollectRange.Value);
        }

        public void OnUpdate(float deltaTime)
        {
            Vector3 heroPos = _hero.Transform.position;

            var allEntities = _lifeContext.Entities;

            for (int i = 0; i < allEntities.Count; i++)
            {
                Entity loot = allEntities[i];

                if (loot.HasComponent<LootTag>() &&
                    loot.CurrentTarget.Value == _hero &&
                    !loot.IsCollected.Value)
                {
                    float sqrDist = (heroPos - loot.Transform.position).sqrMagnitude;

                    if (sqrDist <= _sqrCollectDistance)
                    {
                        Collect(loot);
                    }
                }
            }
        }

        private void UpdateSqrDistance(float newRange)
        {
            float dist = newRange * 0.2f;
            _sqrCollectDistance = dist * dist;
        }

        private void Collect(Entity loot)
        {
            loot.IsCollected.Value = true;

            _hero.LootPickedEvent.Invoke(loot.LootTypeNew.Value);

            Debug.Log($"<color=green>Loot {loot.LootTypeNew.Value} added to hero!</color>");
        }

        public void OnDispose()
        {
            _rangeSubscription?.Dispose();
        }
    }
}