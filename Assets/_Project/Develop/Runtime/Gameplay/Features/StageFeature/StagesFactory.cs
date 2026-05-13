using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Gameplay.Context; 
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using System;

public class StagesFactory
{
    private readonly DIContainer _container;
    private readonly GameplaySceneContext _sceneContext; 

    public StagesFactory(DIContainer container, GameplaySceneContext sceneContext)
    {
        _container = container;
        _sceneContext = sceneContext;
    }

    public IStage Create(StageConfig stageConfig)
    {
        switch (stageConfig)
        {
            case ClearAllEnemiesStageConfig clearAllEnemiesStageConfig:
                return new ClearAllEnemiesStage(
                    clearAllEnemiesStageConfig,
                    _container.Resolve<EnemiesFactory>(),
                    _container.Resolve<EntitiesLifeContext>());

            case FinalPointReachedStageConfig:
                return new FinalPointReachedStage(
                    _container.Resolve<FinalPointTriggerService>(),
                    _container.Resolve<MainHeroHolderService>(),
                    _sceneContext.FinishPoint.position); 

            default:
                throw new ArgumentException(
                    $"Not supported {stageConfig.GetType()} type config");
        }
    }
}