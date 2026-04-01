using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
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

        public LootFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntitiesFactory>();
            _audioService = container.Resolve<AudioService>();
        }

        public Entity Create(LootConfig config, Vector3 position)
        {
            // 1. Создаем базовую сущность
            Entity loot = _entityFactory.CreatePullable(config, position);
            loot.AddLootTag();

            // 2. Генерируем случайный множитель (от 0.5 до 3.0)
            // Он определит и размер, и количество награды
            float randomMultiplier = Random.Range(0.5f, 3f);

            // 3. Инициализируем компоненты (опыт с учетом множителя)
            SetupLootComponents(loot, config, randomMultiplier);

            // 4. Подписываемся на сбор
            loot.IsCollected.Subscribe((oldValue, isCollected) =>
            {
                if (isCollected)
                {
                    OnLootCollected(config, randomMultiplier);
                }
            });

            // 5. Визуальный масштаб и импульс разлета
            ApplyPhysicsAndVisuals(loot, config, randomMultiplier);

            return loot;
        }

        private void SetupLootComponents(Entity loot, LootConfig config, float multiplier)
        {
            switch (config)
            {
                case MemoryShardConfig memoryShardConfig:
                    // Начисляем опыт: база * рандомный множитель
                    float finalExp = memoryShardConfig.ExperienceAmount * multiplier;
                    loot.AddExperienceValue(new ReactiveVariable<float>(finalExp));
                    break;

                default:
                    throw new ArgumentException($"Not support {config.GetType()} type config");
            }
        }

        private void OnLootCollected(LootConfig config, float multiplier)
        {
            // Чем больше объект (multiplier выше), тем ниже будет Pitch (звук "тяжелее")
            // Настраиваем диапазон питча: мелкие (0.5) будут ~1.3, крупные (3.0) будут ~0.7
            float basePitch = 1f;
            float pitchShift = Mathf.Lerp(1.3f, 0.7f, (multiplier - 0.5f) / 2.5f);

            // Добавляем еще капельку рандома для естественности
            float finalPitch = (basePitch * pitchShift) + Random.Range(-0.05f, 0.05f);

            _audioService.PlaySfxByPrefixAuto(config.CollectSoundId, finalPitch);
        }

        private void ApplyPhysicsAndVisuals(Entity loot, LootConfig config, float multiplier)
        {
            // Устанавливаем скейл согласно множителю
            loot.Transform.localScale *= multiplier;

            Rigidbody2D rb = loot.Rigidbody;
            if (loot.BodyCollider != null)
                loot.BodyCollider.isTrigger = false;

            if (rb != null)
            {
                rb.simulated = true;
                rb.gravityScale = Random.Range(config.GravityRange.x, config.GravityRange.y);

                float forceX = Random.Range(config.LaunchForceX.x, config.LaunchForceX.y);
                float forceY = Random.Range(config.LaunchForceY.x, config.LaunchForceY.y);

                // Можно слегка усилить толчок для крупных объектов, чтобы они не падали камнем
                float massWeight = Mathf.Lerp(1f, 1.5f, (multiplier - 0.5f) / 2.5f);
                rb.AddForce(new Vector2(forceX, forceY) * massWeight, ForceMode2D.Impulse);
            }
        }
    }
}