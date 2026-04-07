using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
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

        public StageProcessState(
            StageProviderService stageProviderService,
            LevelProgressService levelProgressService,
            CameraService cameraService,
            FinalPointTriggerService finalPoint,
            InGameTimerFeatureService timerFeature)
        {
            _stageProviderService = stageProviderService;
            _levelProgressService = levelProgressService;
            _cameraService = cameraService;
            _finalPoint = finalPoint;
            _timerFeature = timerFeature;
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
        }

        public void Update(float deltaTime)
        {
            _stageProviderService.UpdateCurrent(deltaTime);
            _levelProgressService.Update(deltaTime);

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Логика "Где мой заказ?!"
            if (Input.GetKeyDown(KeyCode.T))
            {
                // Отдаляем камеру к финишу (зум 11)
                _cameraService.ShowTargetTemporarily(_finalPoint.FinalPointPosition, 11f);
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
            _cameraService.SetZoom(10f);
        }
    }
}