using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class StageProcessState : State, IUpdatableState
    {
        private readonly StageProviderService _stageProviderService;
        private readonly LevelProgressService _levelProgressService;
        private readonly CameraService _cameraService;
        private readonly FinalPointTriggerService _finalPoint;
        private readonly InGameTimerFeatureService _timerFeature;
        private readonly IInputService _inputService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly SceneSwitcherService _sceneSwitcherService;

        private GameplayInputArgs _inputArgs;

        private readonly RankStyleService _rankStyleService;
        private readonly SecretChestCollectService _secretChestService;
        private readonly LevelConfig _levelConfig;

        public StageProcessState(
            StageProviderService stageProviderService,
            LevelProgressService levelProgressService,
            CameraService cameraService,
            FinalPointTriggerService finalPoint,
            InGameTimerFeatureService timerFeature,
            IInputService inputService,
            ICoroutinesPerformer coroutinesPerformer,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs inputArgs,
            RankStyleService rankStyleService,
            SecretChestCollectService secretChestService,
            LevelConfig levelConfig)
        {
            _stageProviderService = stageProviderService;
            _levelProgressService = levelProgressService;
            _cameraService = cameraService;
            _finalPoint = finalPoint;
            _timerFeature = timerFeature;
            _inputService = inputService;
            _coroutinesPerformer = coroutinesPerformer;
            _sceneSwitcherService = sceneSwitcherService;
            _inputArgs = inputArgs;
            _rankStyleService = rankStyleService;
            _secretChestService = secretChestService;
            _levelConfig = levelConfig;
        }

        public override void Enter()
        {
            base.Enter();

            _rankStyleService.Deactivate();
            _secretChestService.Initialize(_levelConfig.SecretChestSpawns.Count);

            _stageProviderService.SwitchToNext();
            _stageProviderService.StartCurrent();

            _timerFeature.Show();
            _inputService.IsEnabled = true;

            _cameraService.StopShowingTarget();
            _cameraService.SetZoom(10f);
        }

        public void Update(float deltaTime)
        {
            _stageProviderService.UpdateCurrent(deltaTime);
            _levelProgressService.Update(deltaTime);
            _timerFeature.Update(deltaTime);
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            if (Input.GetKeyDown(KeyCode.T))
                _cameraService.ShowTargetTemporarily(_finalPoint.FinalPointPosition, 11f);

            if (Input.GetKeyUp(KeyCode.T))
            {
                _cameraService.StopShowingTarget();
                _cameraService.SetZoom(8f);
            }
        }

        public override void Exit()
        {
            base.Exit();

            _stageProviderService.CleanupCurrent();
            _timerFeature.Hide();

            _cameraService.StopShowingTarget();
            _cameraService.SetZoom(8f);
        }
    }
}