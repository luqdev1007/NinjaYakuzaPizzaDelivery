using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    public class EnemySpawnMarker : MonoBehaviour
    {
        private const float MarkerSphereRadius = 0.5f;

        // Альфа общая для всех радиусов: сферы вложены друг в друга, и без
        // прозрачности внешняя просто скрыла бы внутренние.
        private const float RadiusGizmoAlpha = 0.35f;

        private static readonly Color MarkerColor = Color.red;

        // Маркер без назначенного конфига. Такой молча доезжает до рантайма и
        // роняет EnemiesFactory.Create с ArgumentException, поэтому в сцене он
        // обязан бросаться в глаза.
        private static readonly Color MissingConfigColor = Color.magenta;

        private static readonly Color DetectionRadiusColor = new Color(1f, 0.92f, 0.2f, RadiusGizmoAlpha);
        private static readonly Color ArmingRadiusColor = new Color(1f, 0.5f, 0f, RadiusGizmoAlpha);
        private static readonly Color DisarmRadiusColor = new Color(0.35f, 0.7f, 1f, RadiusGizmoAlpha);
        private static readonly Color ExplosionRadiusColor = new Color(1f, 0.15f, 0.1f, RadiusGizmoAlpha);
        private static readonly Color PatrolFallbackRangeColor = new Color(0.4f, 1f, 0.5f, RadiusGizmoAlpha);

        [field: SerializeField] public EntityConfig Config { get; private set; }

        private void OnDrawGizmos()
        {
            DrawMarkerSphere();
            DrawEnemyRadii();
        }

        private void DrawMarkerSphere()
        {
            // Прежний Gizmos.DrawRay(transform.position, transform.forward * 1.5f)
            // убран: в 2D forward смотрит вдоль +Z, то есть в экран, и луч
            // вырождался в точку. Полезной информации он не нёс.
            if (Config == null)
            {
                Gizmos.color = MissingConfigColor;
            }
            else
            {
                Gizmos.color = MarkerColor;
            }

            Gizmos.DrawSphere(transform.position, MarkerSphereRadius);
        }

        private void DrawEnemyRadii()
        {
            if (Config is AngryGhostConfig angryConfig)
            {
                DrawRadius(angryConfig.DetectionRadius, DetectionRadiusColor);
                DrawRadius(angryConfig.ArmingRadius, ArmingRadiusColor);
                DrawRadius(angryConfig.DisarmRadius, DisarmRadiusColor);
                DrawRadius(angryConfig.ExplosionRadius, ExplosionRadiusColor);
            }

            // Слайм. Ветка независима от предыдущей: SlimeConfig наследует
            // EntityConfig напрямую, поэтому pattern matching по GhostConfig его
            // не ловит и порядок веток здесь ни на что не влияет.
            //
            // Рисуется ТОЛЬКО запасной радиус патруля — тот отрезок, по которому
            // слайм пойдёт, если маршрут в сцене не задан. Реальный маршрут
            // рисует SlimePatrolRouteAuthoring, и если он есть, этот радиус
            // не используется вовсе.
            if (Config is SlimeConfig slimeConfig)
            {
                DrawRadius(slimeConfig.PatrolFallbackHalfRange, PatrolFallbackRangeColor);
            }
        }

        private void DrawRadius(float radius, Color color)
        {
            if (radius <= 0f)
            {
                return;
            }

            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
