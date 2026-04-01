using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootFactory
    {
        private readonly EntitiesFactory _entityFactory;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public LootFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntitiesFactory>();
            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>();
        }

        public Entity Create(LootConfig config, Vector3 position)
        {
            // Метод CreatePullable сам инициализирует InSpawnProcess(true)
            Entity loot = _entityFactory.CreatePullable(config.PrefabPath, position);
            loot.AddLootTag();

            var rb = loot.Transform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = true;
                rb.gravityScale = 1.2f; // Чуть тяжелее, чтобы падали быстрее

                // Разлет веером
                float forceX = Random.Range(-5f, 5f);
                float forceY = Random.Range(6f, 9f);
                rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            }

            _entitiesLifeContext.Add(loot);
            return loot;
        }
    }
}