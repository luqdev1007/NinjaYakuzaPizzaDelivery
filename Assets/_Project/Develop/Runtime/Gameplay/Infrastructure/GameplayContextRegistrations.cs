using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Style;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelResultsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Gameplay.Services;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        private static GameplayInputArgs _inputArgs;

        public static void Process(DIContainer container, GameplayInputArgs inputArgs)
        {
            Debug.Log("Process registrations on gameplay scene");
            _inputArgs = inputArgs;

            // --- UI слой ---
            container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();
            container.RegisterAsSingle(CreateGameplayPopupService).NonLazy();
            container.RegisterAsSingle(CreateGameplayScreenPresenter).NonLazy();
            container.RegisterAsSingle(CreateGameplayPresentersFactory);

            // --- Сущности и фабрики ---
            container.RegisterAsSingle(CreateEntitiesFactory).NonLazy();
            container.RegisterAsSingle(CreateEntitiesLifeContext);
            container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();
            container.RegisterAsSingle(CreateMainHeroHolderService).NonLazy();
            container.RegisterAsSingle(CreateCollidersRegistryService);
            container.RegisterAsSingle(CreateBrainsFactory);
            container.RegisterAsSingle(CreateAIBrainContext);
            container.RegisterAsSingle(CreateMainHeroFactory);
            container.RegisterAsSingle(CreateEnemiesFactory);
            container.RegisterAsSingle(CreateStagesFactory);

            // --- Логика уровней ---
            container.RegisterAsSingle(CreateStageProviderService);
            container.RegisterAsSingle(CreateFinalPointTriggerService);
            container.RegisterAsSingle(CreateGameplayStatesFactory);
            container.RegisterAsSingle(CreateGameplayStatesContext);
            container.RegisterAsSingle(CreateStartGameTriggerService);
            container.RegisterAsSingle(CreateLevelProgressService);
            container.RegisterAsSingle(CreateCameraService);

            // --- Лут и прогресс ---
            container.RegisterAsSingle(CreateLootFactory);
            container.RegisterAsSingle(CreateDropLootService);
            container.RegisterAsSingle(CreateInGameTimerFeatureService);
            container.RegisterAsSingle(CreateStyleService);
            container.RegisterAsSingle(CreateStyleEvaluator);
            container.RegisterAsSingle(CreateSecretChestCollectService);
            container.RegisterAsSingle(CreateLevelResultService);
        }

        private static StageProviderService CreateStageProviderService(DIContainer container)
        {
            return new StageProviderService(
                container.Resolve<ILevelStaticDataService>(),
                container.Resolve<StagesFactory>()
            );
        }

        private static StagesFactory CreateStagesFactory(DIContainer container)
        {
            return new StagesFactory(
                container,
                container.Resolve<ILevelStaticDataService>()
            );
        }

        private static LevelResultService CreateLevelResultService(DIContainer container) =>
            new LevelResultService(container.Resolve<RankStyleService>(), container.Resolve<SecretChestCollectService>());

        private static SecretChestCollectService CreateSecretChestCollectService(DIContainer container) =>
            new SecretChestCollectService();

        private static RankStyleService CreateStyleService(DIContainer container)
        {
            var configProvider = container.Resolve<ConfigsProviderService>();
            return new RankStyleService(
                configProvider.GetConfig<StyleRankConfig>(),
                configProvider.GetConfig<StyleActionsConfig>()
            );
        }

        private static StyleEvaluator CreateStyleEvaluator(DIContainer container) =>
            new StyleEvaluator(container.Resolve<RankStyleService>(), container.Resolve<ConfigsProviderService>().GetConfig<StyleActionsConfig>());

        private static InGameTimerFeatureService CreateInGameTimerFeatureService(DIContainer container) =>
            new InGameTimerFeatureService();

        private static DropLootService CreateDropLootService(DIContainer container) =>
            new DropLootService(container.Resolve<LootFactory>());

        private static LootFactory CreateLootFactory(DIContainer container) =>
            new LootFactory(container);

        private static CameraService CreateCameraService(DIContainer container) =>
            new CameraService(Camera.main);

        private static LevelProgressService CreateLevelProgressService(DIContainer container) =>
            new LevelProgressService(container.Resolve<MainHeroHolderService>(), container.Resolve<FinalPointTriggerService>());

        private static StartGameTriggerService CreateStartGameTriggerService(DIContainer container) =>
            new StartGameTriggerService();

        private static GameplayStatesContext CreateGameplayStatesContext(DIContainer container) =>
            new GameplayStatesContext(container.Resolve<GameplayStatesFactory>().CreateGameplayStateMachine(_inputArgs));

        private static GameplayStatesFactory CreateGameplayStatesFactory(DIContainer container) =>
            new GameplayStatesFactory(container, _inputArgs);

        private static MainHeroHolderService CreateMainHeroHolderService(DIContainer container) =>
            new MainHeroHolderService(container.Resolve<EntitiesLifeContext>());

        private static FinalPointTriggerService CreateFinalPointTriggerService(DIContainer container) =>
            new FinalPointTriggerService(container.Resolve<EntitiesFactory>(), container.Resolve<EntitiesLifeContext>());

        private static MainHeroFactory CreateMainHeroFactory(DIContainer container) => new MainHeroFactory(container);

        private static EnemiesFactory CreateEnemiesFactory(DIContainer container) => new EnemiesFactory(container);

        private static AIBrainsContext CreateAIBrainContext(DIContainer container) => new AIBrainsContext();

        private static BrainsFactory CreateBrainsFactory(DIContainer container) => new BrainsFactory(container);

        private static CollidersRegistryService CreateCollidersRegistryService(DIContainer container) => new CollidersRegistryService();

        private static MonoEntitiesFactory CreateMonoEntitiesFactory(DIContainer container) =>
            new MonoEntitiesFactory(container.Resolve<ResourcesAssetsLoader>(), container.Resolve<EntitiesLifeContext>(), container.Resolve<CollidersRegistryService>());

        private static EntitiesLifeContext CreateEntitiesLifeContext(DIContainer container) => new EntitiesLifeContext();

        private static EntitiesFactory CreateEntitiesFactory(DIContainer container) => new EntitiesFactory(container);

        private static GameplayPopupService CreateGameplayPopupService(DIContainer container) =>
            new GameplayPopupService(container.Resolve<ViewsFactory>(), container.Resolve<ProjectPresentersFactory>(), container.Resolve<GameplayUIRoot>(), container.Resolve<GameplayPresentersFactory>());

        private static GameplayScreenPresenter CreateGameplayScreenPresenter(DIContainer container)
        {
            GameplayUIRoot uiRoot = container.Resolve<GameplayUIRoot>();
            GameplayScreenView view = container.Resolve<ViewsFactory>().Create<GameplayScreenView>(ViewIDs.GameplayScreenView, uiRoot.HUDLayer);
            return container.Resolve<GameplayPresentersFactory>().CreateGameplayScreen(view);
        }

        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer container) => new GameplayPresentersFactory(container, _inputArgs);

        private static GameplayUIRoot CreateGameplayUIRoot(DIContainer container)
        {
            ResourcesAssetsLoader loader = container.Resolve<ResourcesAssetsLoader>();
            return Object.Instantiate(loader.Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot"));
        }
    }
}
