using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class AutoDeleteTimerSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;

        public void OnInit(Entity entity)
        {
            _entity = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            // Безопасная проверка: тикаем только если компонент есть и время не вышло
            if (_entity.AutoDeleteCurrentTime.Value > 0)
            {
                _entity.AutoDeleteCurrentTime.Value -= deltaTime;
            }
        }
    }
}