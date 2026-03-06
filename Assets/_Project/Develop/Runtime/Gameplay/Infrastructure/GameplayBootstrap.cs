using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
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

        private CameraFollowService _cameraFollowService;

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

            Entity hero = _container.Resolve<MainHeroFactory>().Create(Vector3.zero);
            _cameraFollowService.SetTarget(hero.Transform);

            yield break;
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
        }

        private void LateUpdate()
        {
            _screenPresenter?.LateUpdate();
        }
    }
}
