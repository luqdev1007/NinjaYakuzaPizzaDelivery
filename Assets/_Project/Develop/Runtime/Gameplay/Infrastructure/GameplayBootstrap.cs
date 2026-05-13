using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Context;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private GameplayInputArgs _inputArgs;
        private GameplayStatesContext _gameplayStatesContext;
        private GameplayScreenPresenter _screenPresenter;
        private EntitiesLifeContext _entitiesLifeContext;
        private AIBrainsContext _brainsContext;
        private GameplaySceneContext _sceneContext;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;

            if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
            {
                throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)} type");
            }

            _inputArgs = gameplayInputArgs;

            // Регистрируем всё, что не зависит от объектов на сцене префаба
            GameplayContextRegistrations.Process(_container, _inputArgs, null);
        }

        public override IEnumerator Initialize()
        {
            var configsProvider = _container.Resolve<ConfigsProviderService>();
            LevelConfig levelConfig = configsProvider.GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            GameObject levelHolder = GameObject.FindWithTag("LevelHolder");

            if (levelHolder == null) 
                throw new NullReferenceException("LevelHolder not found");

            GameObject levelInstance = Instantiate(levelConfig.LevelPrefab, levelHolder.transform);

            _sceneContext = levelInstance.GetComponentInChildren<GameplaySceneContext>();

            if (_sceneContext == null) 
                throw new NullReferenceException("GameplaySceneContext missing in Level Prefab");

            _container.RegisterAsSingle(c => new CameraService(
                _sceneContext.IntroCamera,
                _sceneContext.ScoutingCamera,
                _sceneContext.HeroCamera
            ));

            _screenPresenter = _container.Resolve<GameplayScreenPresenter>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

            var enemiesFactory = _container.Resolve<EnemiesFactory>();
            var lootFactory = _container.Resolve<LootFactory>();

            foreach (var enemyData in _sceneContext.Enemies)
            {
                enemiesFactory.Create(enemyData.Position, enemyData.Config);
            }

            foreach (var chestPos in _sceneContext.Chests)
            {
                lootFactory.CreateSecretChest(chestPos);
            }

            yield break;
        }

        public override void Run()
        {
            _gameplayStatesContext.Run();
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            _brainsContext?.Update(deltaTime);
            _entitiesLifeContext?.Update(deltaTime);
            _gameplayStatesContext?.Update(deltaTime);
        }
    }
}