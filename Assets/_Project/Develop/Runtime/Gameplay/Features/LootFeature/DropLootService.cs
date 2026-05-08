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
            DropLootInternal(entity.Transform.position, lootTable);
        }

        private void DropLootInternal(Vector3 position, LootTableConfig lootTable)
        {
            int attempts = Random.Range(lootTable.TotalDropCountRange.x, lootTable.TotalDropCountRange.y + 1);

            for (int i = 0; i < attempts; i++)
            {
                LootDropEntry entry = GetRandomEntry(lootTable);
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