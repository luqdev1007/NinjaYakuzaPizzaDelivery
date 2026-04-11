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
            // 1. Создаем базовую сущность через общую фабрику
            Entity loot = _entityFactory.CreatePullable(config, position);
            loot.AddLootTag();

            // 2. Генерируем множитель для вариативности (размер и ценность)
            float randomMultiplier = Random.Range(0.5f, 3f);

            // 3. Настраиваем компоненты данных (Coins или Experience)
            SetupLootComponents(loot, config, randomMultiplier);

            // 4. Подписываемся на событие сбора
            loot.IsCollected.Subscribe((oldValue, isCollected) =>
            {
                if (isCollected)
                {
                    HandleLootCollection(loot, config, randomMultiplier);
                }
            });

            // 5. Визуал и физический импульс
            ApplyPhysicsAndVisuals(loot, config, randomMultiplier);

            return loot;
        }

        private void SetupLootComponents(Entity loot, LootConfig config, float multiplier)
        {
            // Расширяемый switch под разные конфиги лута
            if (config is SoulShardLootConfig soulShardLootConfig)
            {
                float finalExp = soulShardLootConfig.ExperienceAmount * multiplier;
                loot.AddExperienceValue(new ReactiveVariable<float>(finalExp));
            }
            else if (config is CoinLootConfig coinConfig) // Если добавишь такой конфиг
            {
                int finalCoins = Mathf.RoundToInt(coinConfig.BaseAmount * multiplier);
                loot.AddCoins(new ReactiveVariable<int>(finalCoins));
            }
            // Можно добавить другие типы (чертежи и т.д.)
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
            // Масштабируем визуальную часть
            loot.Transform.localScale *= multiplier;

            Rigidbody2D rb = loot.Rigidbody;

            // На время разлета выключаем триггер, чтобы он мог сталкиваться с окружением
            if (loot.BodyCollider != null)
                loot.BodyCollider.isTrigger = false;

            if (rb != null)
            {
                rb.simulated = true;
                rb.gravityScale = Random.Range(config.GravityRange.x, config.GravityRange.y);

                float forceX = Random.Range(config.LaunchForceX.x, config.LaunchForceX.y);
                float forceY = Random.Range(config.LaunchForceY.x, config.LaunchForceY.y);

                // Чуть больше массы тяжелым объектам для честного импульса
                float massWeight = Mathf.Lerp(1f, 1.5f, (multiplier - 0.5f) / 2.5f);
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