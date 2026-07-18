using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    /// <summary>
    /// Выставляет IsAgro, когда герой входит в DetectionRadius.
    /// </summary>
    /// <remarks>
    /// Обнаружение вынесено в систему, а не в состояние, сознательно: переход
    /// Wander -> Chase живёт во внешней стейт-машине и читает готовый флаг. Держи
    /// эту проверку внутри вложенной машины блуждания — она бы не работала, пока
    /// активна не та ветка.
    ///
    /// Агро НЕОБРАТИМО по дизайну: подняв флаг, система больше ничего не делает
    /// (ранний выход по IsAgro). Обратного перехода Chase -> Wander нет, и это не
    /// только дизайн — повторный вход во вложенную машину блуждания был бы
    /// некорректен: StateMachine.Enter() не зовёт Enter() текущего состояния
    /// (проверка if (_currentState == null)), поэтому машина возобновилась бы с
    /// невзведённым состоянием.
    ///
    /// Сравнение квадратов расстояний без sqrt — по образцу
    /// NearestDamagableTargetSelector.
    ///
    /// Канал fixed: рядом с ArmingTimerSystem и движением, которое эту дистанцию
    /// и сокращает.
    /// </remarks>
    public class AgroDetectionSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private readonly MainHeroHolderService _mainHeroHolderService;

        private Transform _selfTransform;
        private ReactiveVariable<bool> _isAgro;
        private ReactiveVariable<float> _detectionRadius;

        public AgroDetectionSystem(MainHeroHolderService mainHeroHolderService)
        {
            _mainHeroHolderService = mainHeroHolderService;
        }

        public void OnInit(Entity entity)
        {
            _selfTransform = entity.Transform;
            _isAgro = entity.IsAgro;
            _detectionRadius = entity.DetectionRadius;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_isAgro.Value)
            {
                return;
            }

            Entity mainHero = _mainHeroHolderService.MainHero;

            if (mainHero == null)
            {
                return;
            }

            // Сервис не обнуляет ссылку после смерти героя, а GameObject к этому
            // моменту уже уничтожен.
            if (mainHero.Transform == null)
            {
                return;
            }

            Vector2 offset = mainHero.Transform.position - _selfTransform.position;

            float detectionRadius = _detectionRadius.Value;

            if (offset.sqrMagnitude <= detectionRadius * detectionRadius)
            {
                _isAgro.Value = true;
            }
        }
    }
}
