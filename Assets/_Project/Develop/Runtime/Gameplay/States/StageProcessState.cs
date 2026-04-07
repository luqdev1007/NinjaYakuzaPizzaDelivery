using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

public class StageProcessState : State, IUpdatableState
{
    private readonly StageProviderService _stageProviderService;
    private readonly LevelProgressService _levelProgressService;

    private readonly CameraService _cameraService;
    private readonly FinalPointTriggerService _finalPoint;

    public StageProcessState(
        StageProviderService stageProviderService,
        LevelProgressService levelProgressService,
        CameraService cameraService,
        FinalPointTriggerService finalPoint)
    {
        _stageProviderService = stageProviderService;
        _levelProgressService = levelProgressService;
        _cameraService = cameraService;
        _finalPoint = finalPoint;
    }

    public override void Enter()
    {
        base.Enter();
        _stageProviderService.SwitchToNext();
        _stageProviderService.StartCurrent();
    }

    public void Update(float deltaTime)
    {
        _stageProviderService.UpdateCurrent(deltaTime);
        _levelProgressService.Update(deltaTime);

        // Логика "Где мой заказ?!"
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Показываем финиш с небольшим отдалением (зум 11)
            _cameraService.ShowTargetTemporarily(_finalPoint.FinalPointPosition, 11f);
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            _cameraService.StopShowingTarget();
        }
    }

    public override void Exit()
    {
        base.Exit();
        _stageProviderService.CleanupCurrent();
    }
}