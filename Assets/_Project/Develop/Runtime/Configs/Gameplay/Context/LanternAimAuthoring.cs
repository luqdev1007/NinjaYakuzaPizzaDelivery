using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    /// <summary>
    /// Направление и точку выстрела фонаря дизайнер задаёт прямо в сцене.
    /// </summary>
    /// <remarks>
    /// Вешается на ТОТ ЖЕ GameObject, где висит <see cref="EnemySpawnMarker"/> с
    /// LanternConfig — по образцу <see cref="SlimePatrolRouteAuthoring"/>.
    ///
    /// Muzzle — дочерняя пустышка («дуло»): её МИРОВАЯ ПОЗИЦИЯ это точка вылета,
    /// её локальная ось +X (transform.right) — НАПРАВЛЕНИЕ выстрела. Дизайнер
    /// двигает и ВРАЩАЕТ дуло в сцене, гизмо рисует стрелку направления.
    /// Кастомных инспекторов в проекте нет — трансформ-пустышка + гизмо
    /// единственный способ настройки.
    ///
    /// Компонент СОЗНАТЕЛЬНО без логики: только данные и отрисовка. Снимает
    /// прицел и решает, годится ли он, GameplayBootstrap — там же, где он обходит
    /// маркеры и где уже живёт логирование проблемной расстановки.
    ///
    /// Читается в МИРОВЫХ координатах один раз при спавне — фонарь стационарен,
    /// двигать дуло в рантайме бессмысленно.
    /// </remarks>
    public class LanternAimAuthoring : MonoBehaviour
    {
        private const float ArrowLength = 1.5f;
        private const float ArrowHeadLength = 0.3f;
        private const float ArrowHeadAngle = 25f;
        private const float MissingMuzzleSphereRadius = 0.5f;

        private static readonly Color AimColor = new Color(1f, 0.75f, 0.2f, 0.9f);
        private static readonly Color FireRadiusColor = new Color(1f, 0.5f, 0.15f, 0.5f);

        // Дуло не задано — снаряд полетит вниз из точки спавна, БЕЗ единой ошибки
        // в консоли (warning печатает GameplayBootstrap). Поэтому в сцене такой
        // маркер обязан бросаться в глаза, по образцу MissingPointColor
        // в SlimePatrolRouteAuthoring.
        private static readonly Color MissingMuzzleColor = Color.magenta;

        [field: SerializeField] public Transform Muzzle { get; private set; }

        private void OnDrawGizmos()
        {
            DrawFireRadius();

            if (Muzzle == null)
            {
                Gizmos.color = MissingMuzzleColor;
                Gizmos.DrawWireSphere(transform.position, MissingMuzzleSphereRadius);
                return;
            }

            Vector3 origin = Muzzle.position;
            Vector3 direction = Muzzle.right;

            Gizmos.color = AimColor;

            Vector3 tip = origin + direction * ArrowLength;
            Gizmos.DrawLine(origin, tip);

            // Наконечник стрелки — два отрезка от кончика назад под углом.
            Vector3 back = -direction;
            Vector3 leftHead = Quaternion.Euler(0f, 0f, ArrowHeadAngle) * back * ArrowHeadLength;
            Vector3 rightHead = Quaternion.Euler(0f, 0f, -ArrowHeadAngle) * back * ArrowHeadLength;

            Gizmos.DrawLine(tip, tip + leftHead);
            Gizmos.DrawLine(tip, tip + rightHead);
        }

        // Радиус активации берётся из соседнего EnemySpawnMarker (его Config —
        // LanternConfig): значение живёт в конфиге, а гизмо — на authoring, как
        // просит контракт. Нет маркера/не тот конфиг — радиус просто не рисуем.
        private void DrawFireRadius()
        {
            if (TryGetComponent(out EnemySpawnMarker marker) == false)
            {
                return;
            }

            if (marker.Config is LanternConfig lanternConfig)
            {
                Gizmos.color = FireRadiusColor;
                Gizmos.DrawWireSphere(transform.position, lanternConfig.FireRadius);
            }
        }
    }
}
