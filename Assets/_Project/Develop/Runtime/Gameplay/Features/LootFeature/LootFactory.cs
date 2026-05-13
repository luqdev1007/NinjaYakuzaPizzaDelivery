using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootFactory
    {
        private readonly EntitiesFactory _entityFactory;
        private readonly WalletService _walletService;
        private readonly ConfigsProviderService _configsProvider;

        public LootFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntitiesFactory>();
            _walletService = container.Resolve<WalletService>();
            _configsProvider = container.Resolve<ConfigsProviderService>();
        }

        public void CreateSecretChest(Vector3 position)
        {
            MetaLootConfig config = _configsProvider.GetConfig<MetaLootConfig>();
            Create(config, position);
        }

        public Entity Create(LootConfig config, Vector3 position)
        {
            Entity loot = _entityFactory.CreatePullable(config, position);

            float randomMultiplier = (config is MetaLootConfig) ? 1f : Random.Range(0.5f, 3f);

            SetupLootComponents(loot, config, randomMultiplier);
            ApplyPhysicsAndVisuals(loot, config, randomMultiplier);

            return loot;
        }

        private void SetupLootComponents(Entity loot, LootConfig config, float multiplier)
        {
            if (config is SoulShardLootConfig soulShardLootConfig)
            {
                float finalExp = soulShardLootConfig.ExperienceAmount * multiplier;
                // loot.AddExperienceValue(finalExp);
            }
            else if (config is CoinLootConfig coinConfig)
            {
                int finalCoins = Mathf.RoundToInt(coinConfig.BaseAmount * multiplier);
                // loot.AddCoins(finalCoins);
            }
            else if (config is MetaLootConfig metaConfig)
            {
                // Секретный лут (премиум валюта) обычно не множится
                // loot.AddMetaCurrency(metaConfig.Amount);
            }
        }

        private void ApplyPhysicsAndVisuals(Entity loot, LootConfig config, float multiplier)
        {
            if (config is not MetaLootConfig)
            {
                // loot.Transform.localScale *= multiplier;
            }

            // Rigidbody2D rb = loot.Rigidbody;
            // if (rb != null)
            // {
            //     rb.simulated = true;
            //     float forceX = Random.Range(config.LaunchForceRangeX.x, config.LaunchForceRangeX.y);
            //     float forceY = Random.Range(config.LaunchForceRangeY.x, config.LaunchForceRangeY.y);
            //     rb.AddForce(new Vector2(forceX, forceY) * multiplier, ForceMode2D.Impulse);
            // }
        }

        private CurrencyTypes MapLootToCurrency(LootType lootType)
        {
            return lootType switch
            {
                LootType.SoulShard => CurrencyTypes.SoulShard,
                LootType.Coin => CurrencyTypes.Coins,
                _ => CurrencyTypes.Coins
            };
        }
    }
}