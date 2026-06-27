using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs
{
    public class BuffDistanceCollectSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _lifeContext;
        private readonly BuffService _buffService;

        private Entity _hero;
        private readonly float _collectDistance = 0.4f;

        public BuffDistanceCollectSystem(EntitiesLifeContext lifeContext, BuffService buffService)
        {
            _lifeContext = lifeContext;
            _buffService = buffService;
        }

        public void OnInit(Entity entity)
        {
            _hero = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lifeContext == null)
            {
                return;
            }

            Vector3 heroPosition = _hero.Transform.position;

            for (int i = _lifeContext.Entities.Count - 1; i >= 0; i--)
            {
                Entity buffEntity = _lifeContext.Entities[i];

                if (buffEntity.HasComponent<BuffIsCollected>() == false)
                {
                    continue;
                }

                if (buffEntity.BuffIsCollected.Value)
                {
                    continue;
                }

                float distance = Vector3.Distance(heroPosition, buffEntity.Transform.position);

                if (distance <= _collectDistance)
                {
                    buffEntity.BuffIsCollected.Value = true;

                    _buffService.Pickup(_hero, buffEntity.BuffPickupConfigC.Value);

                    _lifeContext.Release(buffEntity);
                }
            }
        }
    }
}