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
        private readonly StyleEvaluator _styleEvaluator;
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
            StyleEvaluator styleEvaluator,
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
            _styleEvaluator = styleEvaluator;
            _inputService = inputService;
            _startPosition = startPosition;
        }

        public override void Enter()
        {
            base.Enter();

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
            _styleEvaluator.Tick(deltaTime);
            _stageProviderService.UpdateCurrent(deltaTime);
        }

        public override void Exit()
        {
            _timerService.Stop();
            _stageProviderService.CleanupCurrent();
            base.Exit();
        }
    }
}