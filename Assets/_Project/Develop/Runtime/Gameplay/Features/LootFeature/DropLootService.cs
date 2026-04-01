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

        public void DropLootFor(Entity entity, LootTableConfig lootTable)
        {
            if (lootTable == null) return;

            Vector3 spawnPosition = entity.Transform.position;

            // Высыпаем 4-5 штук гарантированно для теста/визуала
            int count = Random.Range(4, 6);

            for (int i = 0; i < count; i++)
            {
                // Берем случайный конфиг из таблицы
                var config = lootTable.PossibleLoot[Random.Range(0, lootTable.PossibleLoot.Count)];
                _lootFactory.Create(config, spawnPosition);
            }
        }
    }
}