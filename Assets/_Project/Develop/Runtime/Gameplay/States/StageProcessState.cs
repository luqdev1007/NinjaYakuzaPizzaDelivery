using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;

public class StageProcessState : State, IUpdatableState
{
    private readonly StageProviderService _stageProviderService;
    private readonly LevelProgressService _levelProgressService;

    public StageProcessState(
        StageProviderService stageProviderService,
        LevelProgressService levelProgressService)
    {
        _stageProviderService = stageProviderService;
        _levelProgressService = levelProgressService;
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
    }

    public override void Exit()
    {
        base.Exit();
        _stageProviderService.CleanupCurrent();
    }
}