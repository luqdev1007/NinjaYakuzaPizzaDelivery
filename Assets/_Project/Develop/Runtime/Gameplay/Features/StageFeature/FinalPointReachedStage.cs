using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

public class FinalPointReachedStage : IStage
{
    private readonly FinalPointTriggerService _finalPointTrigger;
    private readonly LevelProgressService _levelProgressService;
    private readonly MainHeroHolderService _heroHolder;
    private readonly Vector3 _finalPointPosition;
    private readonly ReactiveEvent _completed = new();
    private bool _inProcess;

    public IReadOnlyEvent Completed => _completed;

    public FinalPointReachedStage(
        FinalPointTriggerService finalPointTrigger,
        LevelProgressService levelProgressService,
        MainHeroHolderService heroHolder,
        Vector3 finalPointPosition)
    {
        _finalPointTrigger = finalPointTrigger;
        _levelProgressService = levelProgressService;
        _heroHolder = heroHolder;
        _finalPointPosition = finalPointPosition;
    }

    public void Start()
    {
        _finalPointTrigger.Create(_finalPointPosition);
        _levelProgressService.Initialize(_heroHolder.MainHero.Transform.position);
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
        _levelProgressService.Reset();
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