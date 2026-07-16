using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    public class DamageKnockbackTimerSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _knockbackElapsedTime;
        private ReactiveVariable<float> _knockbackDuration;

        public void OnInit(Entity entity)
        {
            _knockbackElapsedTime = entity.KnockbackElapsedTime;
            _knockbackDuration = entity.KnockbackDuration;
        }


        // Тик knockback-окна. Раньше здесь сравнивался таймер сам с собой
        // (elapsed < elapsed), из-за чего условие никогда не выполнялось и окно
        // не истекало — открывшись один раз, оно висело вечно.
        public void OnUpdate(float deltaTime)
        {
            if (_knockbackElapsedTime.Value < _knockbackDuration.Value)
            {
                _knockbackElapsedTime.Value += deltaTime;
            }
        }
    }
}
