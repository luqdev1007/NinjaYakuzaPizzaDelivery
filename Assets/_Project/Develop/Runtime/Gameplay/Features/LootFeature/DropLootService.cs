using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
            DropLootInternal(entity.Transform.position, lootTable);
        }

        public void DropLootFor(Vector3 spawnPosition, LootTableConfig lootTable)
        {
            if (lootTable == null) return;
            DropLootInternal(spawnPosition, lootTable);
        }

        private void DropLootInternal(Vector3 position, LootTableConfig lootTable)
        {
            // 1. Определяем общее кол-во предметов, которые вылетят
            int attempts = Random.Range(lootTable.TotalDropCount.x, lootTable.TotalDropCount.y + 1);

            for (int i = 0; i < attempts; i++)
            {
                LootDropEntry entry = GetRandomEntry(lootTable);

                if (entry == null || entry.Config == null) continue;

                // 2. Для каждого выбранного типа определяем, сколько штук заспавнить (например, 3 монетки сразу)
                int count = Random.Range(entry.CountRange.x, entry.CountRange.y + 1);

                for (int j = 0; j < count; j++)
                {
                    Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                    _lootFactory.Create(entry.Config, position + offset);
                }
            }
        }

        private LootDropEntry GetRandomEntry(LootTableConfig lootTable)
        {
            if (lootTable.LootEntries == null || lootTable.LootEntries.Count == 0) return null;

            int totalWeight = lootTable.LootEntries.Sum(e => e.Weight);
            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var entry in lootTable.LootEntries)
            {
                currentWeight += entry.Weight;
                if (randomValue < currentWeight)
                    return entry;
            }

            return lootTable.LootEntries[0];
        }
    }
}