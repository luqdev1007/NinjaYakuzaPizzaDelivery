using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    /// <summary>
    /// Точку вылета снаряда фонаря («дуло») дизайнер задаёт прямо в сцене.
    /// </summary>
    /// <remarks>
    /// Вешается на ТОТ ЖЕ GameObject, где висит <see cref="EnemySpawnMarker"/> с
    /// LanternConfig — по образцу <see cref="SlimePatrolRouteAuthoring"/>.
    ///
    /// Muzzle — дочерняя пустышка: её МИРОВАЯ ПОЗИЦИЯ это точка вылета снаряда.
    /// ОРИЕНТАЦИЯ дула НЕ используется — направление выстрела считается в рантайме
    /// снапшотом на героя (см. LanternFireSystem), поэтому дизайнеру достаточно
    /// двигать пустышку, вращать её незачем.
    ///
    /// Гизмо: круг радиуса стрельбы (значение — из соседнего LanternConfig) плюс
    /// точка дула. Стрелки направления больше нет — направление не задаётся в сцене.
    /// Кастомных инспекторов в проекте нет — трансформ-пустышка + гизмо
    /// единственный способ настройки.
    ///
    /// Компонент СОЗНАТЕЛЬНО без логики: только данные и отрисовка. Снимает точку
    /// вылета и решает, годится ли она, GameplayBootstrap — там же, где он обходит
    /// маркеры и где уже живёт логирование проблемной расстановки.
    ///
    /// Читается в МИРОВЫХ координатах один раз при спавне — фонарь стационарен.
    /// </remarks>
    public class LanternMuzzleAuthoring : MonoBehaviour
    {
        private const float MuzzleSphereRadius = 0.12f;
        private const float MissingMuzzleSphereRadius = 0.5f;

        private static readonly Color MuzzleColor = new Color(1f, 0.75f, 0.2f, 0.9f);
        private static readonly Color FireRadiusColor = new Color(1f, 0.5f, 0.15f, 0.5f);

        // Дуло не задано — снаряд полетит из ЦЕНТРА фонаря, БЕЗ ошибки в консоли
        // (warning печатает GameplayBootstrap). Поэтому в сцене такой маркер обязан
        // бросаться в глаза, по образцу MissingPointColor в SlimePatrolRouteAuthoring.
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

            Gizmos.color = MuzzleColor;
            Gizmos.DrawSphere(Muzzle.position, MuzzleSphereRadius);
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
