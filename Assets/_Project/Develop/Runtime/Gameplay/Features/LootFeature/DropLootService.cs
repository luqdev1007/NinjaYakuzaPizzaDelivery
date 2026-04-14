using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class DropLootService
    {
        private readonly LootFactory _lootFactory;

        public DropLootService(LootFactory lootFactory)
        {
            _lootFactory = lootFactory;
        }

        public void DropLootFor(Vector3 spawnPosition, LootTableConfig lootTable)
        {
            if (lootTable == null)
            {
                Debug.Log("No loot table");
                return;
            }

            int count = Random.Range(5, 30);

            for (int i = 0; i < count; i++)
            {
                var config = lootTable.PossibleLoot[Random.Range(0, lootTable.PossibleLoot.Count)]; // рандом лута

                if (config == null || string.IsNullOrEmpty(config.PrefabPath))
                {
                    Debug.LogWarning("DropLootService: Попытка заспавнить пустой лут из таблицы!");
                    continue;
                }

                Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                _lootFactory.Create(config, spawnPosition + offset);
            }
        }

        public void DropLootFor(Entity entity, LootTableConfig lootTable)
        {
            if (lootTable == null)
            {
                Debug.Log("No loot table");
                return;
            }

            Vector3 spawnPosition = entity.Transform.position;

            int count = Random.Range(5, 30);

            for (int i = 0; i < count; i++)
            {
                var config = lootTable.PossibleLoot[Random.Range(0, lootTable.PossibleLoot.Count)]; // рандом лута

                if (config == null || string.IsNullOrEmpty(config.PrefabPath))
                {
                    Debug.LogWarning("DropLootService: Попытка заспавнить пустой лут из таблицы!");
                    continue;
                }

                Vector3 offset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
                _lootFactory.Create(config, spawnPosition + offset);
            }
        }
    }
}