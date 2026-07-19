using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Move
{
    /// <summary>
    /// Едет К ТОЧКЕ, а не в направлении. Отличие от
    /// <see cref="SimpleRigidbodyMovementSystem"/> принципиальное: та рулит по
    /// вектору MoveDirection и никогда сама не останавливается, поэтому для
    /// маршрута с концами не годится — слайм проскакивал бы точку разворота.
    /// </summary>
    /// <remarks>
    /// ИНВАРИАНТ SINGLE-WRITER: эта система — ЕДИНСТВЕННЫЙ писатель
    /// linearVelocity своей сущности в ТИКОВОМ канале. Единственное исключение —
    /// DamageKnockbackSystem, но она пишет в колбэке TakeDamageEvent и на время
    /// своего окна выключает нас через CanMove (KnockbackElapsedTime против
    /// KnockbackDuration), так что одновременной записи не возникает.
    ///
    /// Поэтому на сущность с этой системой НЕЛЬЗЯ вешать
    /// PhysicsStabilizationSystem: она второй тиковый писатель того же поля и
    /// вдобавок гасит скорость к нулю, то есть просто съест движение. Призраку
    /// она нужна, чтобы арка knockback красиво оседала; здесь ту же роль играет
    /// сама рулёжка — по окончании окна мы возвращаем тело к точке маршрута.
    ///
    /// Пишем ОБЕ ОСИ осознанно. Тело живёт с gravityScale = 0 (слой Enemies не
    /// сталкивается с геометрией уровня, см. SlimeConfig), падать ему некуда и
    /// сохранять чужую вертикаль незачем — вертикаль наша целиком.
    ///
    /// ЗАМЕЧАНИЕ ПРО LinearDamping. На префабе демпфирование 10 (значение
    /// призрака, менять не надо). Демпфирование применяется физикой ПОСЛЕ нашей
    /// записи, внутри того же шага, поэтому фактическая скорость перемещения
    /// выходит примерно на 17% ниже MovementSpeed при fixedDeltaTime = 0.02
    /// (множитель 1 / (1 + 10 * 0.02) = 0.833). Это не баг и не потеря: просто
    /// при тюнинге скорости не ищи «пропавшие» проценты в коде — они здесь.
    /// </remarks>
    public class TargetPointMovementSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        // Мёртвая зона знака взгляда по X — та же роль и то же значение, что у
        // LookDirectionDeadzoneX в SimpleRigidbodyMovementSystem. У маршрута,
        // заданного мышью, концы почти никогда не лежат строго горизонтально,
        // и на около-вертикальном участке знак x скакал бы вокруг нуля, дёргая
        // фейсинг каждый кадр.
        private const float LookDirectionDeadzoneX = 0.1f;

        private Rigidbody2D _rigidbody;
        private ICompositeCondition _canMove;

        private ReactiveVariable<Vector2> _moveTargetPoint;
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<bool> _isMoving;
        private ReactiveVariable<float> _lookDirectionX;

        private float _arriveDistance;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _canMove = entity.CanMove;

            _moveTargetPoint = entity.MoveTargetPoint;
            _moveSpeed = entity.MoveSpeed;
            _isMoving = entity.IsMoving;
            _lookDirectionX = entity.LookDirectionX;

            _arriveDistance = entity.PatrolArriveDistance;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            // Ровно как SimpleRigidbodyMovementSystem: выключены — не пишем
            // velocity ВООБЩЕ. Иначе затрём арку knockback, ради которой окно
            // и открывается.
            if (_canMove.Evaluate() == false)
            {
                return;
            }

            if (_rigidbody == null)
            {
                return;
            }

            Vector2 toTarget = _moveTargetPoint.Value - _rigidbody.position;

            if (toTarget.magnitude < _arriveDistance)
            {
                _rigidbody.linearVelocity = Vector2.zero;
                _isMoving.Value = false;

                return;
            }

            Vector2 direction = toTarget.normalized;

            _rigidbody.linearVelocity = direction * _moveSpeed.Value;
            _isMoving.Value = true;

            // Единственный писатель LookDirectionX у этой сущности. Схема взята
            // у SimpleRigidbodyMovementSystem: знак фактического движения, а
            // читает его FlipDirectionSystem и разворачивает localRotation по Y.
            if (Mathf.Abs(direction.x) > LookDirectionDeadzoneX)
            {
                _lookDirectionX.Value = Mathf.Sign(direction.x);
            }
        }
    }
}
