using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootDistanceCollectSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _lifeContext;
        private readonly WalletService _walletService;

        private Entity _hero;
        private float _collectDistance = 0.4f;

        public LootDistanceCollectSystem(EntitiesLifeContext lifeContext, WalletService walletService)
        {
            _lifeContext = lifeContext;
            _walletService = walletService;
        }

        public void OnInit(Entity entity)
        {
            _hero = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lifeContext == null) return;

            Vector3 heroPosition = _hero.Transform.position;

            for (int i = _lifeContext.Entities.Count - 1; i >= 0; i--)
            {
                Entity lootEntity = _lifeContext.Entities[i];

                if (lootEntity.HasComponent<LootIsCollected>() && lootEntity.HasComponent<InSpawnProcess>())
                {
                    if (lootEntity.LootIsCollected.Value == false &&
                        lootEntity.InSpawnProcess.Value == false)
                    {
                        float distance = Vector3.Distance(heroPosition, lootEntity.Transform.position);

                        if (distance <= _collectDistance)
                        {
                            lootEntity.LootIsCollected.Value = true;

                            ApplyLootReward(lootEntity);

                            _lifeContext.Release(lootEntity);

                            Object.Destroy(lootEntity.Transform.gameObject);
                        }
                    }
                }
            }
        }

        private void ApplyLootReward(Entity lootEntity)
        {
            LootTypes type = lootEntity.LootType.Value;
            int count = lootEntity.LootCount.Value;

            CurrencyTypes currencyType = MapLootToCurrency(type);

            _walletService.Add(currencyType, count);
            Debug.Log($"[Loot Пылесос] Схавали лут: {type}, Количество: {count}. Отправлено в кошелек!");
        }

        private CurrencyTypes MapLootToCurrency(LootTypes lootType)
        {
            return lootType switch
            {
                LootTypes.SoulShard => CurrencyTypes.SoulShard,
                LootTypes.Coin => CurrencyTypes.Coins,
                _ => CurrencyTypes.Coins
            };
        }
    }
}