using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class DoubleAttackCooldownSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<float> _currentCooldown;

        public void OnInit(Entity entity)
        {
            _currentCooldown = entity.DoubleAttackCurrentCooldown;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_currentCooldown.Value > 0)
            {
                _currentCooldown.Value -= deltaTime;
            }
        }
    }
}