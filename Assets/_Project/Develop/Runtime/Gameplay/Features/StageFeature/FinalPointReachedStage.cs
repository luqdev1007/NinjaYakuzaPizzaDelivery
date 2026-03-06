using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using UnityEngine;

public class FinalPointReachedStage : IStage
{
    private readonly FinalPointTriggerService _finalPointTrigger;
    private readonly GameplayInputArgs _inputArgs;
    private readonly ReactiveEvent _completed = new();
    private bool _inProcess;

    public IReadOnlyEvent Completed => _completed;

    public FinalPointReachedStage(FinalPointTriggerService finalPointTrigger, GameplayInputArgs inputArgs)
    {
        _finalPointTrigger = finalPointTrigger;
        _inputArgs = inputArgs;
    }

    public void Start()
    {
        _finalPointTrigger.Create(_inputArgs.FinalPointSpawnPosition);
        _inProcess = true;
    }

    public void Update(float deltaTime)
    {
        if (_inProcess == false)
            return;

        _finalPointTrigger.Update(deltaTime);

        if (_finalPointTrigger.HasMainHeroContact.Value)
            ProcessEnd();
    }

    public void Cleanup()
    {
        _finalPointTrigger.Cleanup();
        _inProcess = false;
    }

    public void Dispose() => Cleanup();

    private void ProcessEnd()
    {
        _inProcess = false;
        _completed.Invoke();
    }
}