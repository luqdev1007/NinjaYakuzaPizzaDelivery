using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private GameplayInputArgs _inputArgs;
        private GameplayStatesContext _gameplayStatesContext;
        private GameplayScreenPresenter _screenPresenter;
        private EntitiesLifeContext _entitiesLifeContext;
        private CameraService _cameraService;
        private AIBrainsContext _brainsContext;
        private AudioService _audioService;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
            if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)} type");

            _inputArgs = gameplayInputArgs;
            GameplayContextRegistrations.Process(_container, _inputArgs);
        }

        public override IEnumerator Initialize()
        {
            _screenPresenter = _container.Resolve<GameplayScreenPresenter>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();
            _cameraService = _container.Resolve<CameraService>();
            _audioService = _container.Resolve<AudioService>();

            // Получаем конфиги
            var configsProvider = _container.Resolve<ConfigsProviderService>();
            LevelConfig levelConfig = configsProvider.GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            // Получаем наш новый мастер-провайдер лута
            MasterLootProviderConfig lootProvider = configsProvider.GetConfig<MasterLootProviderConfig>();

            // Настройка камеры
            Vector3 camPosition = levelConfig.StartPlayerPosition;
            camPosition.z = -10;
            Camera.main.transform.position = camPosition;

            // Спавн геометрии уровня
            GameObject levelHolder = GameObject.FindWithTag("LevelHolder");
            Instantiate(levelConfig.LevelPrefab, levelHolder.transform);

            // Настройка границ камеры
            GameObject boundsObj = GameObject.FindWithTag("LevelBounds");
            if (boundsObj != null && boundsObj.TryGetComponent<Collider2D>(out var col))
                _cameraService.SetConstraints(col.bounds);

            // Спавн всех сущностей с передачей соответствующих таблиц лута
            CreateEnemiesOnLevel(levelConfig, lootProvider.EnemyLoot);
            CreateSecretChestsOnLevel(levelConfig, lootProvider.ChestLoot);

            yield break;
        }

        private void CreateEnemiesOnLevel(LevelConfig levelConfig, LootTableConfig enemyLoot)
        {
            IReadOnlyList<Vector3> spawnPoints = levelConfig.EnemySpawns;
            GhostConfig config = _container.Resolve<ConfigsProviderService>().GetConfig<GhostConfig>();
            EnemiesFactory enemiesFactory = _container.Resolve<EnemiesFactory>();

            foreach (Vector3 spawnPoint in spawnPoints)
            {
                // Если ты обновил метод Create, чтобы он принимал таблицу лута:
                // enemiesFactory.Create(spawnPoint, config, enemyLoot);

                // Если пока нет, оставляем так (но лучше прокидывать таблицу)
                enemiesFactory.Create(spawnPoint, config);
            }
        }

        private void CreateSecretChestsOnLevel(LevelConfig levelConfig, LootTableConfig chestLoot)
        {
            IReadOnlyList<Vector3> chestPoints = levelConfig.SecretChestSpawns;
            EntitiesFactory entitiesFactory = _container.Resolve<EntitiesFactory>();

            foreach (Vector3 spawnPoint in chestPoints)
            {
                // Создаем сундук, передавая конкретно таблицу для сундуков
                entitiesFactory.CreateChest(spawnPoint, chestLoot);
            }
        }

        public override void Run()
        {
            if (_inputArgs.IsRestart == false)
                _audioService.StartPlaylist("Gameplay");

            _gameplayStatesContext.Run();
        }

        private void Update()
        {
            _brainsContext?.Update(Time.deltaTime);
            _entitiesLifeContext?.Update(Time.deltaTime);
            _gameplayStatesContext?.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.P))
                _container.Resolve<MainHeroHolderService>().MainHero.CollectRange.Value = 100;

            if (Input.GetKeyDown(KeyCode.O))
                _container.Resolve<MainHeroHolderService>().MainHero.CollectRange.Value = 5;
        }

        private void LateUpdate()
        {
            _cameraService?.Update(Time.deltaTime);
        }
    }
}