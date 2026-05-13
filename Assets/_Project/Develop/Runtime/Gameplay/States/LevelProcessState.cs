using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
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
        private readonly InGameTimerFeatureService _timerService;
        private readonly Vector3 _startPosition;

        public LevelProcessState(
            CameraService cameraService,
            MainHeroFactory heroFactory,
            GameplayUIRoot uiRoot,
            InGameTimerFeatureService timerService,
            Vector3 startPosition)
        {
            _cameraService = cameraService;
            _heroFactory = heroFactory;
            _uiRoot = uiRoot;
            _timerService = timerService;
            _startPosition = startPosition;
        }

        public override void Enter()
        {
            base.Enter();

            var hero = _heroFactory.Create(_startPosition);

            _cameraService.AttachHero(hero.Transform);
            _cameraService.SetState(CameraState.HeroFollow);

            if (_uiRoot.HUDLayer != null)
            {
                _uiRoot.HUDLayer.gameObject.SetActive(true);
            }

            _timerService.Start();
        }

        public override void Exit()
        {
            _timerService.Stop();
            base.Exit();
        }

        public void Update(float deltaTime)
        {
        }
    }
}