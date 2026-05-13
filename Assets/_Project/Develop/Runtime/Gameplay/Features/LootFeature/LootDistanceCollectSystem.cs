using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootDistanceCollectSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _lifeContext;
        private Entity _hero;
        private ReactiveVariable<float> _collectDistance = new();

        public LootDistanceCollectSystem(EntitiesLifeContext lifeContext)
        {
            _lifeContext = lifeContext;
        }

        public void OnInit(Entity entity)
        {
            _hero = entity;
            // _collectDistance.Value = entity.CollectRange.Value * 0.2f;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lifeContext == null)
                return;

            // Vector3 heroPosition = _hero.Transform.position;

            for (int i = 0; i < _lifeContext.Entities.Count; i++)
            {
                Entity entity = _lifeContext.Entities[i];

                /*
                if (entity.HasComponent<LootTag>() && entity.IsCollected.Value == false && entity.InSpawnProcess.Value == false)
                {
                    float distance = Vector3.Distance(heroPosition, entity.Transform.position);

                    if (distance <= _collectDistance.Value)
                    {
                        entity.IsCollected.Value = true;
                        Debug.Log($"Лут собран по дистанции!");
                    }
                }
                */
            }
        }
    }
}