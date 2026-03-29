using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;

public class SleepTimerSystem : IUpdatableSystem, IInitializableSystem
{
    private Entity _entity;

    public void OnInit(Entity entity)
    {
        _entity = entity;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_entity.IsAsleep.Value)
        {
            _entity.SleepTimer.Value -= deltaTime;

            if (_entity.SleepTimer.Value <= 0)
            {
                _entity.IsAsleep.Value = false;
            }
        }
    }
}