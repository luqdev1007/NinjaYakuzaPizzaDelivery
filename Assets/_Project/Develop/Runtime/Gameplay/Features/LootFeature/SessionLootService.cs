using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class SessionLootService
    {
        private readonly Dictionary<LootTypes, ReactiveVariable<int>> _collectedLoot = new();

        public event Action<LootTypes, int, int> OnLootUpdated;

        public void AddLoot(LootTypes type, int amount)
        {
            if (amount <= 0) 
                return;

            if (!_collectedLoot.ContainsKey(type))
                _collectedLoot[type] = new ReactiveVariable<int>(0);

            _collectedLoot[type].Value += amount;

            OnLootUpdated?.Invoke(type, amount, _collectedLoot[type].Value);
        }

        public int GetCollectedAmount(LootTypes type)
        {
            return _collectedLoot.TryGetValue(type, out var reactive) ? reactive.Value : 0;
        }

        public void ClearSession()
        {
            _collectedLoot.Clear();
        }

        public IReadOnlyDictionary<LootTypes, int> GetAllCollected()
        {
            var result = new Dictionary<LootTypes, int>();

            foreach (var pair in _collectedLoot)
                result[pair.Key] = pair.Value.Value;
            return result;
        }
    }
}