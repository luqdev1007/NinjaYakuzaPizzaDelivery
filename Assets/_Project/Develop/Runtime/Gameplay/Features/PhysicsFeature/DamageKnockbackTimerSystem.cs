using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DamageKnockbackTimerSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _knockbackInitialTimer;
        private ReactiveVariable<float> _knockbackTimer;

        public void OnInit(Entity entity)
        {
            _knockbackInitialTimer = entity.KnockbackInitialTimer;
            _knockbackTimer = entity.KnockbackTimer;
        }


        public void OnUpdate(float deltaTime)
        {
            if (_knockbackInitialTimer.Value < _knockbackInitialTimer.Value)
            {
                _knockbackInitialTimer.Value += deltaTime;
            }
        }
    }
}