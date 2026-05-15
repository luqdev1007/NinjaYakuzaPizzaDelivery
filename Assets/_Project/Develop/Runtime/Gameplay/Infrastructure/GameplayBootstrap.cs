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
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private GameplayInputArgs _inputArgs;
        private GameplayStatesContext _gameplayStatesContext;
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

            // Раньше здесь вызывался GameplayContextRegistrations.Process с null. 
            // ТЕПЕРЬ МЫ ЭТОГО НЕ ДЕЛАЕМ.
        }

        public override IEnumerator Initialize()
        {
            var configsProvider = _container.Resolve<ConfigsProviderService>();
            LevelConfig levelConfig = configsProvider.GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

            GameObject levelHolder = GameObject.FindWithTag("LevelHolder");
            if (levelHolder == null)
                throw new NullReferenceException("LevelHolder not found");

            // 1. Создаем уровень
            GameObject levelInstance = Instantiate(levelConfig.LevelPrefab, levelHolder.transform);

            // 2. Достаем контекст сцены
            _sceneContext = levelInstance.GetComponentInChildren<GameplaySceneContext>();
            if (_sceneContext == null)
                throw new NullReferenceException("GameplaySceneContext missing in Level Prefab");

            // 3. ТОЛЬКО ТЕПЕРЬ регистрируем все зависимости, когда у нас есть и конфиги, и объекты на сцене
            GameplayContextRegistrations.Process(_container, _inputArgs, _sceneContext);

            // 4. Инициализируем контейнер (теперь NonLazy создадутся без ошибок, так как _sceneContext валиден)
            _container.Initialize();

            // 5. Разрешаем (Resolve) нужные нам для Update системы
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

            // 6. Спавним начальных сущностей
            var enemiesFactory = _container.Resolve<EnemiesFactory>();
            var lootFactory = _container.Resolve<LootFactory>();

            foreach (var enemyData in _sceneContext.Enemies)
                enemiesFactory.Create(enemyData.Position, enemyData.Config);

            /*
            foreach (var chestPos in _sceneContext.Chests)
                lootFactory.CreateSecretChest(chestPos);
            */

            yield break;
        }

        public override void Run()
        {
            IAudioService audioService = _container.Resolve<IAudioService>();

            // Можно брать ключ прямо из конфига уровня, если добавишь туда поле string MusicKey
            // audioService.PlayMusic(levelConfig.MusicKey); 

            audioService.PlayPlaylist("Gameplay_Playlist");

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