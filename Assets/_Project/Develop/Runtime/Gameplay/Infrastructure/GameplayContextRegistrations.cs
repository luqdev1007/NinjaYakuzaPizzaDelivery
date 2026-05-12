using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
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

            container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();

            container.RegisterAsSingle(CreateGameplayPopupService).NonLazy();

            container.RegisterAsSingle(CreateGameplayScreenPresenter).NonLazy();

            container.RegisterAsSingle(CreateGameplayPresentersFactory);

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

            container.RegisterAsSingle(CreateStageProviderService);
            container.RegisterAsSingle(CreateFinalPointTriggerService);

            container.RegisterAsSingle(CreateGameplayStatesFactory);
            container.RegisterAsSingle(CreateGameplayStatesContext);

            container.RegisterAsSingle(CreateStartGameTriggerService);

            container.RegisterAsSingle(CreateCameraService);

            container.RegisterAsSingle(CreateLootFactory);
            container.RegisterAsSingle(CreateDropLootService);


            container.RegisterAsSingle(CreateInGameTimerFeatureService);

            container.RegisterAsSingle(CreateStyleService);

            container.RegisterAsSingle(CreateStyleEvaluator);

            container.RegisterAsSingle(CreateSecretChestCollectService);

            container.RegisterAsSingle(CreateLevelResultService);
        }

        private static LevelResultService CreateLevelResultService(DIContainer container)
        {
            return new LevelResultService(
                container.Resolve<RankStyleService>(), 
                container.Resolve<SecretChestCollectService>()
                );
        }

        private static SecretChestCollectService CreateSecretChestCollectService(DIContainer container)
        {
            return new SecretChestCollectService();
        }

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

        private static InGameTimerFeatureService CreateInGameTimerFeatureService(DIContainer container)
        {
            return new InGameTimerFeatureService();
        }

        private static DropLootService CreateDropLootService(DIContainer container)
        {
            return new DropLootService(container.Resolve<LootFactory>());
        }

        private static LootFactory CreateLootFactory(DIContainer container)
        {
            return new LootFactory(container);
        }

        private static CameraService CreateCameraService(DIContainer container)
        {
            CameraService camService = new CameraService(Camera.main);

            return camService;
        }

        private static StartGameTriggerService CreateStartGameTriggerService(DIContainer container)
        {
            return new StartGameTriggerService();
        }

        private static GameplayStatesContext CreateGameplayStatesContext(DIContainer container)
        {
            return new GameplayStatesContext(
                container.Resolve<GameplayStatesFactory>()
                .CreateGameplayStateMachine(_inputArgs));
        }

        private static GameplayStatesFactory CreateGameplayStatesFactory(DIContainer container)
        {
            return new GameplayStatesFactory(container, _inputArgs);
        }

        private static MainHeroHolderService CreateMainHeroHolderService(DIContainer container)
        {
            return new MainHeroHolderService(container.Resolve<EntitiesLifeContext>());
        }

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

        private static StagesFactory CreateStagesFactory(DIContainer container)
        {
            return new StagesFactory(
                container,
                container.Resolve<ConfigsProviderService>()
                    .GetConfig<LevelsListConfig>()
                    .GetBy(_inputArgs.LevelNumber));
        }

        private static MainHeroFactory CreateMainHeroFactory(DIContainer container)
        {
            return new MainHeroFactory(container);
        }

        private static EnemiesFactory CreateEnemiesFactory(DIContainer container)
        {
            return new EnemiesFactory(container);
        }



        private static AIBrainsContext CreateAIBrainContext(DIContainer container)
        {
            return new AIBrainsContext();
        }

        private static BrainsFactory CreateBrainsFactory(DIContainer container)
        {
            return new BrainsFactory(container);
        }

        private static CollidersRegistryService CreateCollidersRegistryService(DIContainer container)
        {
            return new CollidersRegistryService();
        }

        private static MonoEntitiesFactory CreateMonoEntitiesFactory(DIContainer container)
        {
            return new MonoEntitiesFactory(
                container.Resolve<ResourcesAssetsLoader>(),
                container.Resolve<EntitiesLifeContext>(),
                container.Resolve<CollidersRegistryService>());
        }

        private static EntitiesLifeContext CreateEntitiesLifeContext(DIContainer container)
        {
            return new EntitiesLifeContext();
        }

        private static EntitiesFactory CreateEntitiesFactory(DIContainer container)
        {
            return new EntitiesFactory(container);
        }

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

            GameplayScreenView view = container
                .Resolve<ViewsFactory>()
                .Create<GameplayScreenView>(ViewIDs.GameplayScreenView, uiRoot.HUDLayer);

            GameplayScreenPresenter presenter = container.Resolve<GameplayPresentersFactory>().CreateGameplayScreen(view);

            return presenter;
        }

        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer container)
        {
            return new GameplayPresentersFactory(container, _inputArgs);
        }

        private static GameplayUIRoot CreateGameplayUIRoot(DIContainer container)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();

            GameplayUIRoot gameplayUIRoot = resourcesAssetsLoader
                .Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot");

            return Object.Instantiate(gameplayUIRoot);
        }
    }
}
