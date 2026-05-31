using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootMagnetSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _lifeContext;

        private Entity _player;
        private ReactiveVariable<float> _collectRange;
        private Transform _playerTransform;

        public LootMagnetSystem(EntitiesLifeContext lifeContext)
        {
            _lifeContext = lifeContext;
        }

        public void OnInit(Entity entity)
        {
            _player = entity;
            _collectRange = entity.LootCollectRange;
            _playerTransform = entity.Transform;
        }

        public void OnUpdate(float deltaTime)
        {
            Vector3 heroPosition = _player.Transform.position;
            float magnetRadius = _player.LootCollectRange.Value;

            for (int i = 0; i < _lifeContext.Entities.Count; i++)
            {
                Entity lootEntity = _lifeContext.Entities[i];

                if (lootEntity.HasComponent<LootIsCollected>() &&
                    lootEntity.HasComponent<CurrentTarget>() &&
                    lootEntity.HasComponent<InSpawnProcess>())
                {
                    if (lootEntity.CurrentTarget.Value == null &&
                        lootEntity.InSpawnProcess.Value == false &&
                        lootEntity.LootIsCollected.Value == false)
                    {
                        float distance = Vector3.Distance(heroPosition, lootEntity.Transform.position);

                        if (distance <= magnetRadius)
                        {
                            lootEntity.CurrentTarget.Value = _player;
                        }
                    }
                }
            }
        }
    }
}