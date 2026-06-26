using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.States
{
    public class LevelProcessState : State, IUpdatableState
    {
        private readonly CameraService _cameraService;
        private readonly MainHeroFactory _heroFactory;
        private readonly GameplayUIRoot _uiRoot;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;
        private readonly InGameTimerFeatureService _timerService;
        private readonly StageProviderService _stageProviderService;
        private readonly RankStyleService _styleService;
        // НОВОЕ: зависимость на инпут-сервис для явного восстановления инварианта на входе.
        private readonly IInputService _inputService;
        private readonly Vector3 _startPosition;

        public LevelProcessState(
            CameraService cameraService,
            MainHeroFactory heroFactory,
            GameplayUIRoot uiRoot,
            GameplayScreenPresenter gameplayScreenPresenter,
            InGameTimerFeatureService timerService,
            StageProviderService stageProviderService,
            RankStyleService styleService,
            IInputService inputService,
            Vector3 startPosition)
        {
            _cameraService = cameraService;
            _heroFactory = heroFactory;
            _uiRoot = uiRoot;
            _gameplayScreenPresenter = gameplayScreenPresenter;
            _timerService = timerService;
            _stageProviderService = stageProviderService;
            _styleService = styleService;
            _inputService = inputService;
            _startPosition = startPosition;
        }

        public override void Enter()
        {
            base.Enter();

            // НОВОЕ: явный сброс инварианта "активный геймплей = инпут всегда включён".
            // Закрывает утечку из непарных Enter/Exit (EndGameState, попапы), которая
            // переживает hard scene reload на рестарте — см. обсуждение в чате.
            _inputService.IsEnabled = true;

            var hero = _heroFactory.Create(_startPosition);

            _cameraService.AttachHero(hero.Transform);
            _cameraService.SetState(CameraState.HeroFollow);

            if (_uiRoot.HUDLayer != null)
            {
                _uiRoot.HUDLayer.gameObject.SetActive(true);
            }

            _gameplayScreenPresenter.StartGameplayHud();

            _timerService.Reset();
            _timerService.Start();

            _styleService.Deactivate();

            _stageProviderService.PrepareFirstStage();
        }

        public void Update(float deltaTime)
        {
            _timerService.Tick(deltaTime);
            _styleService.UpdateDecay(deltaTime);
            _stageProviderService.UpdateCurrent(deltaTime);
        }

        public override void Exit()
        {
            _timerService.Stop();
            _stageProviderService.CleanupCurrent();

            Debug.Log(_timerService.ElapsedTime.Value);

            base.Exit();
        }
    }
}