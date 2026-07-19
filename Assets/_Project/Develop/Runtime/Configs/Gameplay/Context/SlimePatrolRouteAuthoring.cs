using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    /// <summary>
    /// Маршрут патруля наземного врага, заданный дизайнером прямо в сцене.
    /// </summary>
    /// <remarks>
    /// Вешается на ТОТ ЖЕ GameObject, где висит <see cref="EnemySpawnMarker"/>.
    /// PointA/PointB — дочерние пустышки, которые двигаются мышью в сцене.
    /// Кастомных инспекторов в проекте нет и мы их не заводим, поэтому
    /// трансформы-пустышки + гизмо — единственный способ настройки.
    ///
    /// Компонент СОЗНАТЕЛЬНО без логики: только данные и отрисовка. Снимает
    /// маршрут и решает, годится ли он, GameplayBootstrap — там же, где он
    /// обходит маркеры, и там же, где уже живёт логирование проблемной
    /// расстановки.
    ///
    /// Точки читаются в МИРОВЫХ координатах один раз при спавне. Двигать их в
    /// рантайме бессмысленно — сущность их уже скопировала.
    /// </remarks>
    public class SlimePatrolRouteAuthoring : MonoBehaviour
    {
        private const float EndpointSphereRadius = 0.2f;
        private const float MissingPointSphereRadius = 0.5f;

        private static readonly Color RouteColor = new Color(0.35f, 1f, 0.45f, 0.9f);

        // Маршрут с незаданной точкой молча доедет до рантайма и свалится в
        // запасной отрезок вокруг спавна — то есть слайм будет ходить не там,
        // где задумано, БЕЗ единой ошибки в консоли. Поэтому в сцене такой
        // маршрут обязан бросаться в глаза, по образцу MissingConfigColor
        // в EnemySpawnMarker.
        private static readonly Color MissingPointColor = Color.magenta;

        [field: SerializeField] public Transform PointA { get; private set; }
        [field: SerializeField] public Transform PointB { get; private set; }

        private void OnDrawGizmos()
        {
            if (PointA == null || PointB == null)
            {
                DrawMissingRoute();
                return;
            }

            Gizmos.color = RouteColor;
            Gizmos.DrawLine(PointA.position, PointB.position);
            Gizmos.DrawSphere(PointA.position, EndpointSphereRadius);
            Gizmos.DrawSphere(PointB.position, EndpointSphereRadius);
        }

        private void DrawMissingRoute()
        {
            Gizmos.color = MissingPointColor;
            Gizmos.DrawWireSphere(transform.position, MissingPointSphereRadius);

            // Заданный конец всё равно показываем: так сразу видно, КАКОЙ из двух
            // потерян, а не просто что «что-то не так».
            if (PointA != null)
            {
                Gizmos.DrawSphere(PointA.position, EndpointSphereRadius);
            }

            if (PointB != null)
            {
                Gizmos.DrawSphere(PointB.position, EndpointSphereRadius);
            }
        }
    }
}
