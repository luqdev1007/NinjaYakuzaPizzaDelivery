using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System;
using System.Collections;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        [SerializeField] private Vector3 _spawnSpringPosition;

        private DIContainer _container;

        private GameplayInputArgs _inputArgs;

        private GameplayStatesContext _gameplayStatesContext;

        private GameplayScreenPresenter _screenPresenter;

        private EntitiesLifeContext _entitiesLifeContext;

        private CameraFollowService _cameraFollowService;

        private Entity _hero;

        private AIBrainsContext _brainsContext;

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
            Debug.Log("Gameplay scene init");

            _screenPresenter = _container.Resolve<GameplayScreenPresenter>();

            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();

            _brainsContext = _container.Resolve<AIBrainsContext>();

            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

            _cameraFollowService = _container.Resolve<CameraFollowService>();

            _hero = _container.Resolve<MainHeroFactory>().Create(Vector3.zero);
            _cameraFollowService.SetTarget(_hero.Transform);

            // create level objects (service)
            CreateLevelObjects();

            yield break;
        }

        private void CreateLevelObjects()
        {
            Entity spring = _container.Resolve<EntitiesFactory>()
                .CreateSpring(_spawnSpringPosition,
                _container.Resolve<ConfigsProviderService>().GetConfig<SpringConfig>());
        }

        private void CreateGhost()
        {
            Entity ghost = _container.Resolve<EnemiesFactory>()
                .Create(_hero.Transform.position + Vector3.up * 2,
                _container.Resolve<ConfigsProviderService>().GetConfig<GhostConfig>());
        }

        public override void Run()
        {
            Debug.Log($"Start gameplay scene");

            _gameplayStatesContext.Run();
        }

        private void Update()
        {
            _cameraFollowService?.Update(Time.deltaTime);

            _brainsContext?.Update(Time.deltaTime);
            _entitiesLifeContext?.Update(Time.deltaTime);
            _gameplayStatesContext?.Update(Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Equals)) // + 
                Time.timeScale = Mathf.Min(1f, Mathf.Round((Time.timeScale + 0.1f) * 10f) / 10f);

            if (Input.GetKeyDown(KeyCode.Minus)) // -
                Time.timeScale = Mathf.Max(0f, Mathf.Round((Time.timeScale - 0.1f) * 10f) / 10f);

            if (Input.GetKeyDown(KeyCode.G))
                CreateGhost();
        }

        private void LateUpdate()
        {
            _screenPresenter?.LateUpdate();
        }
    }
}
