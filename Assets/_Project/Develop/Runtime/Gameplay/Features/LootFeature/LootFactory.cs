using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.SpawnFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive; 
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootFactory
    {
        private readonly EntitiesFactory _entityFactory;
        private readonly ConfigsProviderService _configsProvider;
        private readonly EntitiesLifeContext _entitiesLifeContext; 

        public LootFactory(DIContainer container)
        {
            _entityFactory = container.Resolve<EntitiesFactory>();
            _configsProvider = container.Resolve<ConfigsProviderService>();
            _entitiesLifeContext = container.Resolve<EntitiesLifeContext>(); 
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

            // 1. ПОДКЛЮЧАЕМ ОБЩУЮ СИСТЕМУ СПАВНА
            loot.AddSystem(new SpawnProcessTimerSystem());

            // 2. Очищенная система старения лута
            loot.AddSystem(new LootLifeCycleSystem(_entitiesLifeContext));

            // 3. Система полета к игроку
            loot.AddSystem(new LootArcMovementSystem(config.TravelTime, config.ArcHeight));

            _entitiesLifeContext.Add(loot);

            return loot;
        }

        private void SetupLootComponents(Entity loot, LootConfig config, float multiplier)
        {
            loot
                .AddLootIsCollected(new ReactiveVariable<bool>(false))
                .AddInSpawnProcess(new ReactiveVariable<bool>(true))
                .AddCurrentTarget(new ReactiveVariable<Entity>(null))

                .AddMoveDirection(new ReactiveVariable<Vector2>(Vector2.zero))
                .AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))

                .AddSpawnCurrentTime(new ReactiveVariable<float>(config.SpawnDuration))
                .AddSpawnInitialTime(new ReactiveVariable<float>(config.SpawnDuration))
                .AddLootInitialLifeTime(new ReactiveVariable<float>(config.LifeTime)) 
                .AddLootCurrentLifeTime(new ReactiveVariable<float>(config.LifeTime))
                ; 

            ICompositeCondition moveCondition = new CompositeCondition()
                .Add(new FuncCondition(() => loot.InSpawnProcess.Value == false))
                ;

            loot.AddCanMove(moveCondition);

            if (config is SoulShardLootConfig soulShardLootConfig)
            {
                int finalExp = Mathf.RoundToInt(soulShardLootConfig.ExperienceAmount * multiplier);
                loot.AddLootCount(new ReactiveVariable<int>(finalExp));
                loot.AddLootType(new ReactiveVariable<LootTypes>(LootTypes.SoulShard));
            }
            else if (config is CoinLootConfig coinConfig)
            {
                int finalCoins = Mathf.RoundToInt(coinConfig.BaseAmount * multiplier);
                loot.AddLootCount(new ReactiveVariable<int>(finalCoins));
                loot.AddLootType(new ReactiveVariable<LootTypes>(LootTypes.Coin));
            }
        }

        private void ApplyPhysicsAndVisuals(Entity loot, LootConfig config, float multiplier)
        {
            if (config is not MetaLootConfig)
            {
                loot.Transform.localScale *= multiplier;
            }

            Rigidbody2D rb = loot.Rigidbody; 

            if (rb != null)
            {
                rb.simulated = true;
                float forceX = Random.Range(config.LaunchForceRangeX.x, config.LaunchForceRangeX.y);
                float forceY = Random.Range(config.LaunchForceRangeY.x, config.LaunchForceRangeY.y);

                rb.AddForce(new Vector2(forceX, forceY) * multiplier, ForceMode2D.Impulse);
            }
        }
    }
}