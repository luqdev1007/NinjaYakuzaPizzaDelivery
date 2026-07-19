using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Patrol
{
    /// <summary>
    /// Маршрут патруля, снятый со сцены и переданный в фабрику.
    /// </summary>
    /// <remarks>
    /// НЕ IEntityComponent, а обычный DTO: живёт ровно на участке
    /// GameplayBootstrap -> EnemiesFactory -> EntitiesFactory и в сущность
    /// уезжает уже разложенным на PatrolPointA / PatrolPointB.
    ///
    /// В EnemiesFactory.Create приезжает как НЕОБЯЗАТЕЛЬНЫЙ nullable-параметр.
    /// Так сделано, чтобы не трогать ClearAllEnemiesStage: у него на руках
    /// только позиция и конфиг, маркера в сцене нет вовсе — он молча попадает
    /// в ветку «маршрут не задан» и получает запасной отрезок от позиции спавна.
    /// </remarks>
    public readonly struct PatrolRoute
    {
        public readonly Vector2 PointA;
        public readonly Vector2 PointB;

        public PatrolRoute(Vector2 pointA, Vector2 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}
