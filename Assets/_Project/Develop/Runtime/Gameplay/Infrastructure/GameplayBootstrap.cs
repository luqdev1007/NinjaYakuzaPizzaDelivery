using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.MainHeroes;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.Features.Ballista;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using System.Linq;
using Unity.Cinemachine;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities.Projectiles;

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
        private ProjectilesFactory _projectilesFactory;
        private VehiclesFactory _vehiclesFactory;
        private MainHeroesFactory _mainHeroesFactory;

        // Main Entities
        private Entity _mainShip;
        private Entity _captain;
        private Entity _wizard;
        private Entity _engineer;
        private Entity _ballista;

        // UI
        private GameplayScreenPresenter _gameplayScreenPresenter;

        // other
        private BallistaController _ballistaController;
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
            _vehiclesFactory = _container.Resolve<VehiclesFactory>();
            _mainHeroesFactory = _container.Resolve<MainHeroesFactory>();
            _projectilesFactory = _container.Resolve<ProjectilesFactory>();

            // ctx
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

            // UI
            _gameplayScreenPresenter = _container.Resolve<GameplayScreenPresenter>();

            // Input
            _input = _container.Resolve<IInputService>();

            CreateMainCrew(); // init

            yield break;
        }

        public override void Run()
        {
            Debug.Log("Старт геймплейной сцены");

            // UI
            _gameplayScreenPresenter.SubscribeHealthViewToEntity(_mainShip);

            _gameplayStatesContext.Run();
        }

        private void Update()
        {
            _gameplayStatesContext?.Update(Time.deltaTime);

            _entitiesLifeContext?.Update(Time.deltaTime);

            _brainsContext?.Update(Time.deltaTime);

            if (_input != null && _input.IsAttackKeyReleased)
            {
                BallistaAttack();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                KillAllCrew();
            }
        }

        private void BallistaAttack()
        {
            if (_ballistaController == null)
                return;

            float ballistaPower = _ballistaController.ShootPower;
            float launchPowerMultiplier = _ballistaController.ChargeProgress < 0.5f ? 1f : _ballistaController.ChargeProgress * 2f;
            SimpleProjectileConfig config = _container.Resolve<ConfigsProviderService>().GetConfig<SimpleProjectileConfig>();
            config.GravityScale = 10;

            _projectilesFactory.Create(
                _ballistaController.ProjectileParent,
                new ProjectileCreationContext(_ballista,
                launchPower: ballistaPower * launchPowerMultiplier,
                finalDamage: 2,
                launchDelay: 0.25f,
                shootDirection: _ballistaController.ProjectileParent.forward),
                config);
        }

        private void CreateMainCrew()
        {
            // main ship
            MainShipConfig config = _container.Resolve<ConfigsProviderService>()
                .GetConfig<MainShipConfig>(); // saved data provide

            _mainShip = _vehiclesFactory.Create(null, Teams.Allies, config);

            ShipPlace[] shipPlaces = _mainShip.Transform.GetComponentsInChildren<ShipPlace>();

            // ballista
            BallistaConfig ballistaConfig = _container.Resolve<ConfigsProviderService>().GetConfig<BallistaConfig>();
            Transform ballistaSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Ballista).transform;
            _ballista = _entitiesFactory.CreateBallista(ballistaSpawnPointParent, ballistaConfig, Teams.Allies);

            _ballistaController = _ballista.Transform.GetComponent<BallistaController>();
            _ballistaController.Init(_container.Resolve<IInputService>());

            _mainShip.Transform.GetComponentInChildren<CinemachineCamera>().Target.TrackingTarget = _ballistaController.CameraPivot;

            // captain
            Transform captainSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Driver).transform;
            _captain = _mainHeroesFactory.CreateCaptain(captainSpawnPointParent);

            // wizard
            Transform wizardSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Mast).transform;
            _wizard = _mainHeroesFactory.CreateWizard(wizardSpawnPointParent);

            // engineer
            Transform engineerSpawnPointParent = shipPlaces.First(i => i.PlaceType == ShipPlaceType.Paluba).transform;
            _engineer = _mainHeroesFactory.CreateEngineer(engineerSpawnPointParent);
            _engineer.Transform.GetComponent<ConfigurableJoint>().connectedBody = _ballistaController.EngineerPivot;
        }

        private void KillAllCrew()
        {
            _captain.CurrentHealth.Value = 0;
            _wizard.CurrentHealth.Value = 0;
            _engineer.CurrentHealth.Value = 0;
        }
    }
}