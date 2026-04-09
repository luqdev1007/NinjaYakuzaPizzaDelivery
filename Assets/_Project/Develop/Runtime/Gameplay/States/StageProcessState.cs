using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
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

        public StageProcessState(
            StageProviderService stageProviderService,
            LevelProgressService levelProgressService,
            CameraService cameraService,
            FinalPointTriggerService finalPoint,
            InGameTimerFeatureService timerFeature,
            IInputService inputService,
            ICoroutinesPerformer coroutinesPerformer,
            SceneSwitcherService sceneSwitcherService,
            GameplayInputArgs inputArgs)
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
        }

        public override void Enter()
        {
            base.Enter();

            // Переключаем стейдж и запускаем его логику
            _stageProviderService.SwitchToNext();
            _stageProviderService.StartCurrent();

            // Даем команду сервису-посреднику показать таймер
            // Презентер поймает это событие и сам запустит анимацию/отсчет
            _timerFeature.Show();
            _inputService.IsEnabled = true;

            _cameraService.StopShowingTarget();
            _cameraService.SetZoom(10f);
        }

        public void Update(float deltaTime)
        {
            _stageProviderService.UpdateCurrent(deltaTime);
            _levelProgressService.Update(deltaTime);

            HandleCameraInput();

            if (_inputService.IsRestartKeyPressed)
            {
                _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, _inputArgs));
            }
        }

        private void HandleCameraInput()
        {
            // Логика "Где мой заказ?!"
            if (Input.GetKeyDown(KeyCode.T))
            {
                // Отдаляем камеру к финишу (зум 11)
                _cameraService.ShowTargetTemporarily(_finalPoint.FinalPointPosition, 14f);
            }

            if (Input.GetKeyUp(KeyCode.T))
            {
                // Возвращаем слежку за героем
                _cameraService.StopShowingTarget();

                // Сбрасываем зум к стандартному игровому значению
                _cameraService.SetZoom(10f);
            }
        }

        public override void Exit()
        {
            base.Exit();

            _stageProviderService.CleanupCurrent();

            // Даем команду скрыть таймер
            _timerFeature.Hide();

            // На всякий случай сбрасываем камеру, чтобы зум не залип при переходе в Win/Defeat
            _cameraService.StopShowingTarget();

            // Сбрасываем зум к стандартному игровому значению
            _cameraService.SetZoom(10f);
        }
    }
}