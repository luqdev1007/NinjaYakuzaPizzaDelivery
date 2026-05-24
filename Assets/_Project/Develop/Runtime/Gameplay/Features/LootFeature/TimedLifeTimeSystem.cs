using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class TimedLifeTimeSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _lifeTime;

        public void OnInit(Entity entity)
        {
            _lifeTime = entity.LifeTime;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lifeTime.Value > 0)
            {
                _lifeTime.Value -= deltaTime;
            }
        }
    }
}