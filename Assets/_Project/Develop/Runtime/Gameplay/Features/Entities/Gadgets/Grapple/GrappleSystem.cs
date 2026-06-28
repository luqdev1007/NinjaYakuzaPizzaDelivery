using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.ThrowableFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleSystem : IInitializableSystem, IUpdatableSystem
    {
        // CHANGED: фаза притягивания переехала из корутины (WaitForFixedUpdate) в OnUpdate.
        // Раньше intent обрабатывался в Update (переменный таймстеп), а тяга — в корутине на фиксированном.
        // Этот раскол давал рассинхрон по кадрам, двойные StopPulling и "вязкое" ощущение.
        // Теперь вся механика тикает в одном месте, как у остальных movement-систем.

        // --- тюнинг (кандидаты на вынос в GrappleHookConfig) ---

        // Гравитация во время притягивания — оставляет лёгкую "воздушную" дугу.
        private const float PullGravity = 0.2f;

        // CHANGED: ускорение тяги как доля от максимальной скорости.
        // Выше — резче выходит на максималку (snappy), ниже — плавнее.
        private const float PullAccelerationFactor = 6f;

        // CHANGED: множитель от MaxFlyDistance, на котором принудительно срываемся
        // (якорь уехал слишком далеко / нас пронесло мимо).
        private const float BreakDistanceFactor = 1.5f;

        // CHANGED: лёгкий доворот вверх при отпускании — помогает цепляться за уступы.
        // Поставь 0, если не нравится, что отпуск чуть задирает траекторию вверх.
        private const float ReleaseUpBias = 0.35f;

        // CHANGED: минимальная скорость вылета, чтобы совсем короткий грэпл всё равно ощущался.
        private const float MinReleaseSpeed = 12f;

        private enum GrappleState
        {
            Idle,
            Firing,
            Pulling
        }

        private readonly GrappleHookConfig _grappleHookConfig;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private ICompositeCondition _canGrapple;
        private ReactiveVariable<bool> _intentGrapple;
        private ReactiveVariable<bool> _isGrappling;
        private ReactiveVariable<Transform> _grappleHookTransform;
        private ReactiveVariable<Vector3> _grappleHookAnchor;
        private ReactiveEvent _grappleAnchoredEvent;
        private ReactiveVariable<Vector2> _intentAimDirection;

        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private GrappleHookProjectile _activeProjectile; // CHANGED: типизирован напрямую, убрана мёртвая else-ветка
        private GrappleState _state;

        // CHANGED: якорь храним явно вместо состояния внутри корутины.
        private Collider2D _anchorCollider;
        // Локальная точка на коллайдере — корректно следует за движущимися платформами.
        private Vector2 _anchorLocalOffset;
        private Vector2 _lastPullDirection;

        private float _defaultGravity;
        private bool _wasGrappleIntendedLastFrame;

        public GrappleSystem(GrappleHookConfig grappleHookConfig, ICoroutinesPerformer performer)
        {
            _grappleHookConfig = grappleHookConfig;
            _coroutinesPerformer = performer;
        }

        public void OnInit(Entity entity)
        {
            _canGrapple = entity.CanGrapple;
            _intentGrapple = entity.IntentGrapple;
            _isGrappling = entity.IsGrappling;
            _grappleHookTransform = entity.GrappleHookTransform;
            _grappleHookAnchor = entity.GrappleAnchorPoint;
            _intentAimDirection = entity.IntentAimDirection;
            _grappleAnchoredEvent = entity.GrappleAnchoredEvent;

            _transform = entity.Transform;
            _rigidbody = entity.Rigidbody;

            _defaultGravity = entity.BaseGravityScale.Value;
            _state = GrappleState.Idle;
        }

        public void OnUpdate(float deltaTime)
        {
            bool currentIntent = _intentGrapple.Value;
            bool isPressedDown = currentIntent && !_wasGrappleIntendedLastFrame;
            bool isReleased = !currentIntent && _wasGrappleIntendedLastFrame;
            _wasGrappleIntendedLastFrame = currentIntent;

            switch (_state)
            {
                case GrappleState.Idle:
                    if (isPressedDown && _canGrapple.Evaluate())
                    {
                        TryLaunch();
                    }
                    break;

                case GrappleState.Firing:
                    // CHANGED: отпустили до зацепа — отменяем летящий крюк без инерции.
                    if (isReleased)
                    {
                        Release(applyInertia: false);
                    }
                    break;

                case GrappleState.Pulling:
                    if (isReleased)
                    {
                        // CHANGED: ранний отпуск сохраняет набранную инерцию (по дизайну — Spider-Man / Ghostrunner).
                        Release(applyInertia: true);
                    }
                    else
                    {
                        UpdatePull(deltaTime);
                    }
                    break;
            }
        }

        private void TryLaunch()
        {
            Vector3 dir = new Vector3(_intentAimDirection.Value.x, _intentAimDirection.Value.y, 0f);

            _state = GrappleState.Firing;
            _isGrappling.Value = true;

            _activeProjectile = new GrappleHookProjectile(_grappleHookConfig, _coroutinesPerformer);

            // CHANGED: именованные хендлеры вместо замыканий — чтобы можно было явно отписаться (без утечек).
            _activeProjectile.OnAnchored += HandleAnchored;
            _activeProjectile.OnCompleted += HandleProjectileCompleted;

            _activeProjectile.Launch(_transform.position, dir);

            if (_activeProjectile.Instance != null)
            {
                _grappleHookTransform.Value = _activeProjectile.Instance.transform;
            }
        }

        private void HandleAnchored(Vector2 anchorPos, Collider2D hit)
        {
            // CHANGED: якорь в локальных координатах коллайдера — едет вместе с движущейся платформой.
            _anchorCollider = hit;
            _anchorLocalOffset = (Vector2)hit.transform.InverseTransformPoint(anchorPos);

            _grappleHookTransform.Value = null;
            _rigidbody.gravityScale = PullGravity;

            _state = GrappleState.Pulling;
            _grappleAnchoredEvent?.Invoke();
        }

        private void HandleProjectileCompleted()
        {
            // CHANGED: крюк долетел и не зацепился — реагируем только если ещё в фазе полёта.
            if (_state == GrappleState.Firing)
            {
                Release(applyInertia: false);
            }
        }

        private void UpdatePull(float deltaTime)
        {
            // CHANGED: якорь потерян (коллайдер уничтожен) — срыв с текущей инерцией.
            if (_anchorCollider == null || _anchorCollider.transform == null)
            {
                Release(applyInertia: true);
                return;
            }

            Vector2 anchorWorld = (Vector2)_anchorCollider.transform.TransformPoint(_anchorLocalOffset);
            Vector2 toAnchor = anchorWorld - (Vector2)_transform.position; // CHANGED: убран бессмысленный _transform?.
            float dist = toAnchor.magnitude;

            _grappleHookAnchor.Value = anchorWorld;

            // CHANGED: пронесло слишком далеко / якорь уехал — срыв.
            if (dist > _grappleHookConfig.MaxFlyDistance * BreakDistanceFactor)
            {
                Release(applyInertia: true);
                return;
            }

            // CHANGED: долетели — отпуск с инерцией, никакого зависания у точки (было *0.98 каждый шаг).
            if (dist <= _grappleHookConfig.ArriveDistance)
            {
                if (dist > 0.0001f)
                {
                    _lastPullDirection = toAnchor / dist;
                }
                Release(applyInertia: true);
                return;
            }

            Vector2 dir = toAnchor / dist;
            _lastPullDirection = dir;

            // CHANGED: ускоряем вектор скорости к якорю поверх текущего импульса —
            // сохраняется перпендикулярная составляющая → естественные дуги/качели.
            float maxPullSpeed = _grappleHookConfig.GrappleSpeed; // ВНИМАНИЕ: теперь это макс. скорость, не сила — перетюнь.
            float pullAcceleration = maxPullSpeed * PullAccelerationFactor;

            Vector2 velocity = _rigidbody.linearVelocity;
            velocity += dir * pullAcceleration * deltaTime;

            // CHANGED: кап общей скорости вместо AddForce(Force) — детерминированно, без зависимости от массы.
            if (velocity.magnitude > maxPullSpeed)
            {
                velocity = velocity.normalized * maxPullSpeed;
            }

            _rigidbody.linearVelocity = velocity;
        }

        private void Release(bool applyInertia)
        {
            if (_state == GrappleState.Idle)
            {
                return;
            }

            _rigidbody.gravityScale = _defaultGravity;

            if (applyInertia && _state == GrappleState.Pulling)
            {
                // CHANGED: инерция базируется на РЕАЛЬНОМ векторе скорости (это и есть набранный импульс),
                // а не на принудительном направлении к якорю — сохраняет дугу при раннем отпуске.
                Vector2 currentVelocity = _rigidbody.linearVelocity;
                float currentSpeed = currentVelocity.magnitude;

                Vector2 baseDirection;
                if (currentSpeed > 0.01f)
                {
                    baseDirection = currentVelocity / currentSpeed;
                }
                else
                {
                    baseDirection = _lastPullDirection;
                }

                Vector2 launchDirection = (baseDirection + Vector2.up * ReleaseUpBias).normalized;
                float finalSpeed = Mathf.Max(currentSpeed, MinReleaseSpeed);
                _rigidbody.linearVelocity = launchDirection * finalSpeed;
            }

            CancelAll();
        }

        private void CancelAll()
        {
            if (_activeProjectile != null)
            {
                _activeProjectile.OnAnchored -= HandleAnchored; // CHANGED: явная отписка — без утечек.
                _activeProjectile.OnCompleted -= HandleProjectileCompleted;
                _activeProjectile.Cancel();
                _activeProjectile = null;
            }

            _anchorCollider = null;
            _isGrappling.Value = false;
            _grappleHookTransform.Value = null;
            _grappleHookAnchor.Value = Vector2.zero;

            _state = GrappleState.Idle;
        }
    }
}