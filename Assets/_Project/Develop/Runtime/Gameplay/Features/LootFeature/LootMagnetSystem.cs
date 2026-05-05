using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootMagnetSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly CollidersRegistryService _collidersRegistry;
        private Entity _entity;

        private readonly Collider2D[] _lootBuffer = new Collider2D[20];
        private ContactFilter2D _lootFilter;

        public LootMagnetSystem(CollidersRegistryService collidersRegistry)
        {
            _collidersRegistry = collidersRegistry;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;

            _lootFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayersAPI.LayerMaskLoot
            };
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.Transform == null) 
                return;

            int count = Physics2D.OverlapCircle(
                _entity.Transform.position,
                _entity.CollectRange.Value,
                _lootFilter,
                _lootBuffer);

            for (int i = 0; i < count; i++)
            {
                Entity loot = _collidersRegistry.GetBy(_lootBuffer[i]);

                if (loot == null) 
                    continue;

                if (loot.HasComponent<LootTag>() &&
                    loot.InSpawnProcess.Value == false &&
                    loot.CurrentTarget.Value == null)
                {
                    ActivateMagnet(loot);
                }
            }
        }

        private void ActivateMagnet(Entity loot)
        {
            loot.CurrentTarget.Value = _entity;

            if (loot.Rigidbody != null)
                loot.Rigidbody.simulated = false;

            if (loot.BodyCollider != null)
                loot.BodyCollider.isTrigger = true;
        }
    }
}