using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StagesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Timers;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayContextRegistrations
    {
        private static GameplayInputArgs _inputArgs;

        public static void Process(DIContainer container, GameplayInputArgs args)
        {
            _inputArgs = args;

            // entities
            container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();
            container.RegisterAsSingle(CreateEntitiesFactory);
            container.RegisterAsSingle(CreateEntitiesLifeContext);
            container.RegisterAsSingle(CreateCollidersRegistryService);

            // main heroes
            container.RegisterAsSingle(CreateMainHeroHolderService).NonLazy();
            container.RegisterAsSingle(CreateMainHeroesFactory);

            // input
            container.RegisterAsSingle<IInputService>(CreateDesktopInput);

            // factories
            container.RegisterAsSingle(CreateEnemiesFactory);
            container.RegisterAsSingle(CreateProjectilesFactory);
            container.RegisterAsSingle(CreateStagesFactory);
            container.RegisterAsSingle(CreateGameplayStatesFactory);

            // UI
            container.RegisterAsSingle(CreateGameplayScreenPresenter).NonLazy();
            container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();
            container.RegisterAsSingle(CreateGameplayPresentersFactory);
            container.RegisterAsSingle(CreateGameplayPopupService);

            // AI
            container.RegisterAsSingle(CreateBrainsFactory);
            container.RegisterAsSingle(CreateAIBrainsContext);

            // Stage
            container.RegisterAsSingle(CreateStageProviderService);
            container.RegisterAsSingle(CreateGameplayStatesContext);
            container.RegisterAsSingle(CreateGameplayTimersService);
        }

        private static GameplayTimersService CreateGameplayTimersService(DIContainer container)
        {
            return new GameplayTimersService(container.Resolve<TimerServiceFactory>());
        }

        private static GameplayStatesContext CreateGameplayStatesContext(DIContainer container)
        {
            return new GameplayStatesContext(
                container.Resolve<GameplayStatesFactory>()
                .CreateGameplayStateMachine(_inputArgs));
        }

        private static GameplayStatesFactory CreateGameplayStatesFactory(DIContainer container)
        {
            return new GameplayStatesFactory(container, container.Resolve<GameplayTimersService>());
        }

        private static StageProviderService CreateStageProviderService(DIContainer container)
        {
            return new StageProviderService(
                _inputArgs.LevelConfig,
                container.Resolve<StagesFactory>()
                );
        }

        private static StagesFactory CreateStagesFactory(DIContainer container)
        {
            return new StagesFactory(container);
        }

        private static ProjectilesFactory CreateProjectilesFactory(DIContainer container)
        {
            return new ProjectilesFactory(container);
        }

        private static EnemiesFactory CreateEnemiesFactory(DIContainer container)
        {
            return new EnemiesFactory(container);
        }

        private static MainHeroHolderService CreateMainHeroHolderService(DIContainer container)
        {
            return new MainHeroHolderService(container.Resolve<EntitiesLifeContext>());
        }

        private static MainHeroesFactory CreateMainHeroesFactory(DIContainer container)
        {
            return new MainHeroesFactory(container);
        }

        private static AIBrainsContext CreateAIBrainsContext(DIContainer container)
        {
            return new AIBrainsContext();
        }

        private static BrainsFactory CreateBrainsFactory(DIContainer container)
        {
            return new BrainsFactory(container);
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

        private static GameplayUIRoot CreateGameplayUIRoot(DIContainer container)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();

            GameplayUIRoot mainMenuUIRoot = resourcesAssetsLoader
                .Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot");

            return Object.Instantiate(mainMenuUIRoot);
        }

        private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer container)
        {
            return new GameplayPresentersFactory(container, _inputArgs);
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

        private static DesktopInput CreateDesktopInput(DIContainer container)
        {
            return new DesktopInput();
        }

        private static CollidersRegistryService CreateCollidersRegistryService(DIContainer container)
        {
            return new CollidersRegistryService();
        }

        private static MonoEntitiesFactory CreateMonoEntitiesFactory(DIContainer c)
        {
            return new MonoEntitiesFactory(
                c.Resolve<ResourcesAssetsLoader>(),
                c.Resolve<EntitiesLifeContext>(),
                c.Resolve<CollidersRegistryService>());
        }

        private static EntitiesLifeContext CreateEntitiesLifeContext(DIContainer c)
        {
            return new EntitiesLifeContext();
        }

        private static EntitiesFactory CreateEntitiesFactory(DIContainer c)
        {
            return new EntitiesFactory(c);
        }
    }
}