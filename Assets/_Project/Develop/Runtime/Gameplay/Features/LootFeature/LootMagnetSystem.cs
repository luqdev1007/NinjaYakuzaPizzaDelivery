using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootMagnetSystem : IInitializableSystem, IUpdatableSystem
    {
        private CollidersRegistryService _collidersRegistryService;

        public LootMagnetSystem(CollidersRegistryService collidersRegistryService)
        {
            _collidersRegistryService = collidersRegistryService;
        }

        private Entity _lootMagnetOwner;
        private ReactiveVariable<float> _collectRange;
        private ReactiveEvent<LootType> _lootPickedEvent;
        private Transform _transform;

        public void OnInit(Entity entity)
        {
            _lootMagnetOwner = entity;
            /*
            _collectRange = entity.CollectRange;
            _transform = entity.Transform;
            _lootPickedEvent = entity.LootPickedEvent;
            */
        }

        public void OnUpdate(float deltaTime)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _collectRange.Value, LayersAPI.LayerMaskLoot);

            foreach (Collider2D collider in colliders)
            {
                Entity entity = _collidersRegistryService.GetBy(collider);

                if (entity == null) 
                    continue;

                /*
                if (entity.HasComponent<LootTag>() && entity.HasComponent<CurrentTarget>() && entity.InSpawnProcess.Value == false)
                    Collect(entity);
                */
            }
        }

        private void Collect(Entity loot)
        {
            /*
            loot.CurrentTarget.Value = _lootMagnetOwner;
            loot.BodyCollider.isTrigger = true;

            var rb = loot.Transform.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.simulated = false;

            _lootPickedEvent?.Invoke(LootType.Coin); // ? rework
            */
        }
    }
}