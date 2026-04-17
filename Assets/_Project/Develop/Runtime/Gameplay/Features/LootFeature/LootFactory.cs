using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootFactory
    {
        private readonly EntitiesFactory _entityFactory;
        private readonly AudioService _audioService;
        private readonly WalletService _walletService;

        public LootFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntitiesFactory>();
            _audioService = container.Resolve<AudioService>();
            _walletService = container.Resolve<WalletService>();
        }

        public Entity Create(LootConfig config, Vector3 position)
        {
            Entity loot = _entityFactory.CreatePullable(config, position);
            loot.AddLootTag();

            float randomMultiplier = Random.Range(0.5f, 3f);

            SetupLootComponents(loot, config, randomMultiplier);

            loot.IsCollected.Subscribe((oldValue, isCollected) =>
            {
                if (isCollected)
                {
                    HandleLootCollection(loot, config, randomMultiplier);
                }
            });

            ApplyPhysicsAndVisuals(loot, config, randomMultiplier);

            return loot;
        }

        private void SetupLootComponents(Entity loot, LootConfig config, float multiplier)
        {
            if (config is SoulShardLootConfig soulShardLootConfig)
            {
                float finalExp = soulShardLootConfig.ExperienceAmount * multiplier;
                loot.AddExperienceValue(new ReactiveVariable<float>(finalExp));
            }
            else if (config is CoinLootConfig coinConfig)
            {
                int finalCoins = Mathf.RoundToInt(coinConfig.BaseAmount * multiplier);
                loot.AddCoins(new ReactiveVariable<int>(finalCoins));
            }
        }

        private void HandleLootCollection(Entity loot, LootConfig config, float multiplier)
        {
            // 1. Определяем количество награды из компонентов сущности
            int amountToAdd = 0;

            if (loot.HasComponent<ExperienceValue>())
                amountToAdd = Mathf.RoundToInt(loot.ExperienceValue.Value);
            else if (loot.HasComponent<Coins>())
                amountToAdd = loot.Coins.Value;

            // 2. Начисляем в глобальный кошелек (через маппинг типов)
            CurrencyTypes currencyType = MapLootToCurrency(config.LootType);
            _walletService.Add(currencyType, amountToAdd);

            // 3. Отыгрываем звук с динамическим питчем
            PlayCollectSound(config, multiplier);

            // Тут можно будет вызвать событие для UI-эффектов (полет иконок)
        }

        private void PlayCollectSound(LootConfig config, float multiplier)
        {
            // Мелкий лут — высокий звук, крупный — низкий/тяжелый
            float pitchShift = Mathf.Lerp(1.3f, 0.7f, (multiplier - 0.5f) / 2.5f);
            float finalPitch = pitchShift + Random.Range(-0.05f, 0.05f);

            _audioService.PlaySfxByPrefixAuto(config.CollectSoundId, finalPitch);
        }

        private void ApplyPhysicsAndVisuals(Entity loot, LootConfig config, float multiplier)
        {
            // Проверяем, является ли конфиг мета-лутом (секретным)
            // Если это НЕ секретный лут, применяем множитель скейла
            if (config is not MetaLootConfig)
            {
                loot.Transform.localScale *= multiplier;
            }
            else
            {
                // Для секретного лута можно принудительно поставить 1, 
                // чтобы он всегда выглядел как задумано в префабе
                loot.Transform.localScale = Vector3.one;
            }

            Rigidbody2D rb = loot.Rigidbody;

            if (loot.BodyCollider != null)
                loot.BodyCollider.isTrigger = false;

            if (rb != null)
            {
                rb.simulated = true;
                rb.gravityScale = Random.Range(config.GravityRange.x, config.GravityRange.y);

                float forceX = Random.Range(config.LaunchForceX.x, config.LaunchForceX.y);
                float forceY = Random.Range(config.LaunchForceY.x, config.LaunchForceY.y);

                // Для секретного лута тоже можно убрать влияние множителя на физику, 
                // чтобы он вылетал предсказуемо
                float finalMultiplier = (config is MetaLootConfig) ? 1f : multiplier;
                float massWeight = Mathf.Lerp(1f, 1.5f, (finalMultiplier - 0.5f) / 2.5f);

                rb.AddForce(new Vector2(forceX, forceY) * massWeight, ForceMode2D.Impulse);
            }
        }

        private CurrencyTypes MapLootToCurrency(LootType lootType)
        {
            // Маппинг твоего Enum лута на Enum кошелька
            return lootType switch
            {
                LootType.SoulShard => CurrencyTypes.SoulShard, // Твои "Осколки памяти"
                LootType.Coin => CurrencyTypes.Coins,
                _ => CurrencyTypes.Coins // Дефолт
            };
        }
    }
}