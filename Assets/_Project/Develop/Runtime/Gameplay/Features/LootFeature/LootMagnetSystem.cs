using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootMagnetSystem : IUpdatableSystem, IInitializableSystem
    {
        private Entity _player;

        private readonly float _magnetRadius = 5f;
        private readonly float _pullSpeed = 12f;

        public void OnInit(Entity entity)
        {
            _player = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            // Ищем все сущности с LootTag, которые еще не собраны
            // Если Distance(loot, player) < _magnetRadius
            // Переключаем их в состояние "Collecting"
            // Двигаем Transform лута к Transform игрока через MoveTowards или Lerp
        }
    }
}