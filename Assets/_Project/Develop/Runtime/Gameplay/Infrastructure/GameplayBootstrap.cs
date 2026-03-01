using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
    public class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;

        private GameplayInputArgs _inputArgs;

        // ctx
        private EntitiesLifeContext _entitiesLifeContext;
        private AIBrainsContext _brainsContext;
        private GameplayStatesContext _gameplayStatesContext;

        // factories
        private EntitiesFactory _entitiesFactory;

        // UI
        private GameplayScreenPresenter _gameplayScreenPresenter;

        // Input
        private IInputService _input;

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
            Debug.Log("Инициализация геймплейной сцены");

            // factories
            _entitiesFactory = _container.Resolve<EntitiesFactory>();

            // ctx
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

            // UI
            _gameplayScreenPresenter = _container.Resolve<GameplayScreenPresenter>();

            // Input
            _input = _container.Resolve<IInputService>();

            CreateMainHero(); // init

            yield break;
        }

        public override void Run()
        {
            Debug.Log("Старт геймплейной сцены");

            _gameplayStatesContext.Run();
        }

        private void Update()
        {
            _gameplayStatesContext?.Update(Time.deltaTime);

            _entitiesLifeContext?.Update(Time.deltaTime);

            _brainsContext?.Update(Time.deltaTime);
        }

        private void CreateMainHero()
        {
            // example
            /*
            MainShipConfig config = _container.Resolve<ConfigsProviderService>()
                .GetConfig<MainShipConfig>(); // saved data provide

            _mainShip = _vehiclesFactory.Create(null, Teams.Allies, config);
            */
        }
    }
}