using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Stats;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.AssetsManagment;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataManagment;
using Assets._Project.Develop.Runtime.Utilites.DataManagment.DataRepository;
using Assets._Project.Develop.Runtime.Utilites.DataManagment.KeyStorage;
using Assets._Project.Develop.Runtime.Utilites.DataManagment.Serializers;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Infrastructure.EntryPoint
{
    public class ProjectContextRegistrations
    {
        public static void Process(DIContainer container)
        {
            container.RegisterAsSingle<ICoroutinesPerformer>(CreateCoroutinesPerformer);

            container.RegisterAsSingle(CreateConfigProviderService);

            container.RegisterAsSingle(CreateResourcesAssetsLoader);

            container.RegisterAsSingle(CreateSceneLoaderService);

            container.RegisterAsSingle(CreateSceneSwitcherService);

            container.RegisterAsSingle<ILoadingScreen>(CreateStandartLoadingScreen);

            container.RegisterAsSingle(CreateWalletService).NonLazy();


            container.RegisterAsSingle(CreatePlayerDataProvider);

            container.RegisterAsSingle<ISaveLoadService>(CreateSaveLoadService);

            container.RegisterAsSingle(CreateGameStatsService).NonLazy();

            container.RegisterAsSingle(CreateProjectPresentersFactory);

            container.RegisterAsSingle(CreateViewsFactory);

            container.RegisterAsSingle(CreateTimerServiceFactory);

            container.RegisterAsSingle(CreateLevelsProgressionService).NonLazy();

            container.RegisterAsSingle(CreateAudioService);
        }

        private static AudioService CreateAudioService(DIContainer container)
        {
            var assetsLoader = container.Resolve<ResourcesAssetsLoader>();
            var configProvider = container.Resolve<ConfigsProviderService>();

            AudioConfig config = configProvider.GetConfig<AudioConfig>();
            AudioManager prefab = assetsLoader.Load<AudioManager>("Utilities/AudioManager");

            AudioManager managerInstance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(managerInstance);

            // Достаем миксер из менеджера (убедись, что в AudioManager.cs есть публичное поле Mixer 
            // или достань его через outputAudioMixerGroup.audioMixer)
            var mixer = managerInstance.MusicMixer;

            return new AudioService(config, managerInstance, mixer);
        }

        private static TimerServiceFactory CreateTimerServiceFactory(DIContainer container)
            => new TimerServiceFactory(container);

        private static LevelsProgressionService CreateLevelsProgressionService(DIContainer container)
            => new LevelsProgressionService(container.Resolve<PlayerDataProvider>());

        private static ViewsFactory CreateViewsFactory(DIContainer container) 
            => new ViewsFactory(container.Resolve<ResourcesAssetsLoader>());

        private static ProjectPresentersFactory CreateProjectPresentersFactory(DIContainer container) 
            => new ProjectPresentersFactory(container);

        private static GameStatsService CreateGameStatsService(DIContainer container) 
            => new GameStatsService(container.Resolve<PlayerDataProvider>());

        private static PlayerDataProvider CreatePlayerDataProvider(DIContainer container) 
            => new PlayerDataProvider(container.Resolve<ISaveLoadService>(), container.Resolve<ConfigsProviderService>());

        private static SaveLoadService CreateSaveLoadService(DIContainer container)
        {
            IDataSerializer dataSerializer = new JsonSerializer();
            IDataKeysStorage dataKeysStorage = new MapDataKeysStorage();

            string saveFolderPath = Application.isEditor? Application.dataPath : Application.persistentDataPath;

            IDataRepository dataRepository = new LocalFileDataRepository(saveFolderPath, "json");

            return new SaveLoadService(dataSerializer, dataKeysStorage, dataRepository);
        }

        private static WalletService CreateWalletService(DIContainer container)
        {
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies = new();
            
            foreach (CurrencyTypes currencyType in Enum.GetValues(typeof(CurrencyTypes)))
            {
                currencies[currencyType] = new ReactiveVariable<int>(0);
            }

            return new WalletService(currencies, container.Resolve<PlayerDataProvider>());
        }

        private static SceneSwitcherService CreateSceneSwitcherService(DIContainer container) 
            => new SceneSwitcherService(
                container.Resolve<SceneLoaderService>(), 
                container.Resolve<ILoadingScreen>(), 
                container, 
                container.Resolve<ICoroutinesPerformer>());

        private static SceneLoaderService CreateSceneLoaderService(DIContainer container) 
            => new SceneLoaderService();

        private static ConfigsProviderService CreateConfigProviderService(DIContainer container)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();
            ResourcesConfigsLoader resourcesConfigsLoader = new ResourcesConfigsLoader(resourcesAssetsLoader);

            return new ConfigsProviderService(resourcesConfigsLoader);
        }

        private static ResourcesAssetsLoader CreateResourcesAssetsLoader(DIContainer container) 
            => new ResourcesAssetsLoader();

        private static CoroutinesPerformer CreateCoroutinesPerformer(DIContainer container)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();

            CoroutinesPerformer coroutinesPerformer = resourcesAssetsLoader
                .Load<CoroutinesPerformer>("Utilities/CoroutinesPerformer");

            return Object.Instantiate(coroutinesPerformer);
        }

        private static CoolLoadingScreen CreateStandartLoadingScreen(DIContainer container)
        {
            ResourcesAssetsLoader resourcesAssetsLoader = container.Resolve<ResourcesAssetsLoader>();

            CoolLoadingScreen standartLoadingScreen = resourcesAssetsLoader
                .Load<CoolLoadingScreen>("Utilities/StandartLoadingScreen");

            return Object.Instantiate(standartLoadingScreen);
        }
    }
}