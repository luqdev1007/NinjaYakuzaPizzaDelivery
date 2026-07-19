using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Tongue
{
    /// <summary>
    /// Рисует язык двумя точками LineRenderer: корень — TongueOriginPoint (точка
    /// вылета на слайме, её каждый тик обновляет TongueSystem), кончик — позиция
    /// собственного трансформа, которую двигает та же система.
    ///
    /// Образец — GrappleRopeView. Отличие: здесь ровно две точки, без волны, и
    /// корень читается из компонента, а не берётся с локального Transform —
    /// сущность языка отвязана от слайма и родителя не имеет.
    ///
    /// LineRenderer работает в мировых координатах (useWorldSpace = 1), поэтому
    /// обе точки ставятся как есть, без перевода в локальные.
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class TongueView : EntityView
    {
        private const int RootPointIndex = 0;
        private const int TipPointIndex = 1;
        private const int PointCount = 2;

        [SerializeField] private LineRenderer _lineRenderer;

        private IReadOnlyVariable<Vector2> _originPoint;

        private void OnValidate() => _lineRenderer ??= GetComponent<LineRenderer>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _originPoint = entity.TongueOriginPoint;

            if (_lineRenderer == null)
            {
                return;
            }

            _lineRenderer.positionCount = PointCount;
        }

        private void LateUpdate()
        {
            if (_lineRenderer == null)
            {
                return;
            }

            if (_originPoint == null)
            {
                return;
            }

            _lineRenderer.SetPosition(RootPointIndex, _originPoint.Value);
            _lineRenderer.SetPosition(TipPointIndex, transform.position);
        }
    }
}
