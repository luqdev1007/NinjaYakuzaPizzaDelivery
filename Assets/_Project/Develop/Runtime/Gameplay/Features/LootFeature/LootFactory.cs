using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootFactory
    {
        private readonly EntitiesFactory _entityFactory;

        public LootFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntitiesFactory>();
        }

        public Entity Create(LootConfig config, Vector3 position)
        {
            Entity loot = _entityFactory.CreatePullable(config.PrefabPath, position);
            loot.AddLootTag();

            Rigidbody2D rigidbody = loot.Rigidbody;
            loot.BodyCollider.isTrigger = false;

            if (rigidbody != null)
            {
                rigidbody.simulated = true;
                rigidbody.gravityScale = Random.Range(4, 6);

                float forceX = Random.Range(-7f, 7f); 
                float forceY = Random.Range(6f, 10f); 

                rigidbody.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            }

            return loot;
        }
    }
}