using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjects.Buffs
{
    public class BuffMagnetSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _lifeContext;

        private Entity _hero;

        public BuffMagnetSystem(EntitiesLifeContext lifeContext)
        {
            _lifeContext = lifeContext;
        }

        public void OnInit(Entity entity)
        {
            _hero = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector3 heroPosition = _hero.Transform.position;
            float magnetRadius = _hero.LootCollectRange.Value;

            for (int i = 0; i < _lifeContext.Entities.Count; i++)
            {
                Entity buffEntity = _lifeContext.Entities[i];

                if (buffEntity.HasComponent<BuffIsCollected>() == false ||
                    buffEntity.HasComponent<CurrentTarget>() == false)
                {
                    continue;
                }

                if (buffEntity.CurrentTarget.Value != null ||
                    buffEntity.BuffIsCollected.Value)
                {
                    continue;
                }

                float distance = Vector3.Distance(heroPosition, buffEntity.Transform.position);

                if (distance <= magnetRadius)
                {
                    buffEntity.CurrentTarget.Value = _hero;
                }
            }
        }
    }
}