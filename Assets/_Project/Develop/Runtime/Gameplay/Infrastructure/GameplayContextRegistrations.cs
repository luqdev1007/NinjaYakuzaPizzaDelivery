using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using Assets._Project.Develop.Runtime.Gameplay.Context;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.HitStop;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelResultsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        private static GameplayInputArgs _inputArgs;
        private static GameplaySceneContext _sceneContext;

        public static void Process(DIContainer container, GameplayInputArgs inputArgs, GameplaySceneContext sceneContext)
        {
            Debug.Log("Process registrations on gameplay scene");

            _inputArgs = inputArgs;
            _sceneContext = sceneContext;

            // UI
            container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();
            container.RegisterAsSingle(CreateGameplayPopupService).NonLazy();
            container.RegisterAsSingle(CreateGameplayScreenPresenter).NonLazy();
            container.RegisterAsSingle(CreateGameplayPresentersFactory);

            // Core Entities
            container.RegisterAsSingle(CreateEntitiesFactory).NonLazy();
            container.RegisterAsSingle(CreateEntitiesLifeContext);
            container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();

            // Hero & Collisions
            container.RegisterAsSingle(CreateMainHeroHolderService).NonLazy();
            container.RegisterAsSingle(CreateCollidersRegistryService);

            // AI
            container.RegisterAsSingle(CreateBrainsFactory);
            container.RegisterAsSingle(CreateAIBrainContext);

            // Factories
            container.RegisterAsSingle(CreateMainHeroFactory);
            container.RegisterAsSingle(CreateEnemiesFactory);
            container.RegisterAsSingle(CreateStagesFactory);
            container.RegisterAsSingle(CreateProjectileFactory);

            // Level Systems
            container.RegisterAsSingle(CreateStageProviderService);
            container.RegisterAsSingle(CreateFinalPointTriggerService);
            container.RegisterAsSingle(CreateStartGameTriggerService);

            // Gameplay State Machine
            container.RegisterAsSingle(CreateGameplayStatesFactory);
            container.RegisterAsSingle(CreateGameplayStatesContext);

            // Features (Loot, Style, Timers)
            container.RegisterAsSingle(CreateLootFactory);
            container.RegisterAsSingle(CreateDropLootService);
            container.RegisterAsSingle(CreateInGameTimerFeatureService);
            container.RegisterAsSingle(CreateStyleService);
            container.RegisterAsSingle(CreateStyleEvaluator);
            container.RegisterAsSingle(CreateSecretChestCollectService);
            container.RegisterAsSingle(CreateLevelResultService);

            // hit effect
            container.RegisterAsSingle(CreateHitStopService);

            // session loot
            container.RegisterAsSingle(CreateSessionLootService).NonLazy();

            // Camera
            container.RegisterAsSingle(CreateCameraService);
        }

        private static SessionLootService CreateSessionLootService(DIContainer container)
        {
            return new SessionLootService();
        }


        private static ProjectileFactory CreateProjectileFactory(DIContainer container)
        {
            return new ProjectileFactory(container);
        }

        private static HitStopService CreateHitStopService(DIContainer container)
        {
            return new HitStopService();
        }

        private static CameraService CreateCameraService(DIContainer container)
        {
            return new CameraService(_sceneContext.IntroCamera, _sceneContext.ScoutingCamera, _sceneContext.HeroCamera);
        }

        private static LevelResultService CreateLevelResultService(DIContainer container)
        {
            return new LevelResultService(
                container.Resolve<RankStyleService>(),
                container.Resolve<SecretChestCollectService>()
                );
        }

        private static SecretChestCollectService CreateSecretChestCollectService(DIContainer container) => new SecretChestCollectService();

        private static RankStyleService CreateStyleService(DIContainer container)
        {
            var configProvider = container.Resolve<ConfigsProviderService>();
            return new RankStyleService(
                configProvider.GetConfig<StyleRankConfig>(),
                configProvider.GetConfig<StyleActionsConfig>()
            );
        }

        private static StyleEvaluator CreateStyleEvaluator(DIContainer container)
        {
            return new StyleEvaluator(
                container.Resolve<RankStyleService>(),
                container.Resolve<ConfigsProviderService>().GetConfig<StyleActionsConfig>()
            );
        }

        private static InGameTimerFeatureService CreateInGameTimerFeatureService(DIContainer container) => new InGameTimerFeatureService();

        private static DropLootService CreateDropLootService(DIContainer container) => new DropLootService(container.Resolve<LootFactory>());

        private static LootFactory CreateLootFactory(DIContainer container) => new LootFactory(container);

        private static StartGameTriggerService CreateStartGameTriggerService(DIContainer container) => new StartGameTriggerService();

        private static GameplayStatesContext CreateGameplayStatesContext(DIContainer container)
        {
            return new GameplayStatesContext(container.Resolve<GameplayStatesFactory>().CreateGameplayStateMachine());
        }

        private static GameplayStatesFactory CreateGameplayStatesFactory(DIContainer container) => new GameplayStatesFactory(container, _inputArgs);

        private static MainHeroHolderService CreateMainHeroHolderService(DIContainer container) => new MainHeroHolderService(container.Resolve<EntitiesLifeContext>());

        private static FinalPointTriggerService CreateFinalPointTriggerService(DIContainer container)
        {
            return new FinalPointTriggerService(
                container.Resolve<EntitiesFactory>(),
                container.Resolve<EntitiesLifeContext>());
        }

        private static StageProviderService CreateStageProviderService(DIContainer container)
        {
            return new StageProviderService(
                container.Resolve<ConfigsProviderService>().GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber),
                container.Resolve<StagesFactory>()
                );
        }

        private static StagesFactory CreateStagesFactory(DIContainer container) => new StagesFactory(container, _sceneContext);

        private static MainHeroFactory CreateMainHeroFactory(DIContainer container) => new MainHeroFactory(container);

        private static EnemiesFactory CreateEnemiesFactory(DIContainer container) => new EnemiesFactory(container);

        private static AIBrainsContext CreateAIBrainContext(DIContainer container) => new AIBrainsContext();

        private static BrainsFactory CreateBrainsFactory(DIContainer container) => new BrainsFactory(container);

        private static CollidersRegistryService CreateCollidersRegistryService(DIContainer container) => new CollidersRegistryService();

        private static MonoEntitiesFactory CreateMonoEntitiesFactory(DIContainer container)
        {
            return new MonoEntitiesFactory(
                container.Resolve<ResourcesAssetsLoader>(),
                container.Resolve<EntitiesLifeContext>(),
                container.Resolve<CollidersRegistryService>(),
                container.Resolve<IAudioService>());
        }

        private static EntitiesLifeContext CreateEntitiesLifeContext(DIContainer container) => new EntitiesLifeContext();

        private static EntitiesFactory CreateEntitiesFactory(DIContainer container) => new EntitiesFactory(container);

        private static GameplayPopupService CreateGameplayPopupService(DIContainer container)
        {
            return new GameplayPopupService(
                container.Resolve<ViewsFactory>(),
                container.Resolve<ProjectPresentersFactory>(),
                container.Resolve<GameplayUIRoot>(),
                container.Resolve<GameplayPresentersFactory>()
                );
        }

        private static GameplayScreenPresenter CreateGameplayScreenPresenter(DIContainer container)
        {
            GameplayUIRoot uiRoot = container.Resolve<GameplayUIRoot>();
            GameplayScreenView view = container.Resolve<ViewsFactory>().Create<GameplayScreenView>(ViewIDs.GameplayScreenView, uiRoot.HUDLayer);
            return container.Resolve<GameplayPresentersFactory>().CreateGameplayScreen(view);
        }

        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer container) => new GameplayPresentersFactory(container, _inputArgs);

        private static GameplayUIRoot CreateGameplayUIRoot(DIContainer container)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();
            GameplayUIRoot gameplayUIRoot = resourcesAssetsLoader.Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot");
            return Object.Instantiate(gameplayUIRoot);
        }
    }
}