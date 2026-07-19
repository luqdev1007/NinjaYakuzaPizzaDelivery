using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.RandomManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    /// <summary>
    /// Ходьба между двумя точками маршрута с паузами на концах.
    /// </summary>
    /// <remarks>
    /// ОДНО состояние на весь ping-pong, а не два (A и B) с переходами между
    /// ними. Обоснование: состояние владеет и текущей целью, и таймером паузы
    /// само, поэтому от семантики повторного Enter() не зависит вовсе. Заодно
    /// корневая машина остаётся пустой — под состояния этапа 3 (прицеливание,
    /// выстрел языком, притяг) место чистое.
    ///
    /// Для протокола: StateMachine.SwitchState зовёт Enter() нового состояния
    /// ВСЕГДА, то есть возврат A -> B -> A отработал бы корректно. Не переигрывает
    /// Enter() только повторный вход в саму МАШИНУ (проверка _currentState == null
    /// в StateMachine.Enter). Так что двухсоставный вариант был возможен —
    /// он просто не нужен.
    ///
    /// Собственный float-таймер, а НЕ TimerService: тот живёт на корутинах и
    /// тикает Time.deltaTime на кадровой частоте, что уже числится техдолгом
    /// в TIMESTEP_MIGRATION_PLAN. Здесь таймер тикается тем deltaTime, который
    /// пришёл в Update, а мозги тикаются на fixed — пауза измеряется в тех же
    /// шагах, что и движение.
    ///
    /// Случайность — только из засеянного IGameplayRandom. Длительность паузы
    /// определяет, где слайм окажется в момент X, то есть это реплей-чувствительный
    /// путь наравне с направлением блуждания призрака.
    /// </remarks>
    public class PatrolState : State, IUpdatableState
    {
        private readonly IGameplayRandom _random;

        private readonly Rigidbody2D _rigidbody;
        private readonly ReactiveVariable<Vector2> _moveTargetPoint;

        private readonly Vector2 _pointA;
        private readonly Vector2 _pointB;

        private readonly float _pauseMin;
        private readonly float _pauseMax;
        private readonly float _arriveDistance;

        private bool _isMovingToB;
        private bool _isPaused;
        private float _pauseTimer;

        public PatrolState(Entity entity, IGameplayRandom random)
        {
            _random = random;

            _rigidbody = entity.Rigidbody;
            _moveTargetPoint = entity.MoveTargetPoint;

            _pointA = entity.PatrolPointA;
            _pointB = entity.PatrolPointB;

            _pauseMin = entity.PatrolPauseMin;
            _pauseMax = entity.PatrolPauseMax;
            _arriveDistance = entity.PatrolArriveDistance;

            // Стартовая цель — ДАЛЬНЯЯ из двух, потому что ближняя считалась бы
            // достигнутой сразу же: слайм спавнится на маркере, а маркер дизайнер
            // обычно ставит на один из концов. Идти к ближней означало бы
            // отстоять паузу на старте и только потом поехать.
            _isMovingToB = GetSqrDistanceToStart(_pointB) >= GetSqrDistanceToStart(_pointA);
        }

        public override void Enter()
        {
            base.Enter();

            _moveTargetPoint.Value = GetCurrentTarget();
        }

        public void Update(float deltaTime)
        {
            if (_isPaused)
            {
                TickPause(deltaTime);
            }
            else
            {
                TryStartPauseOnArrival();
            }

            // Цель пишется одной строкой в конце тика, каким бы путём мы сюда ни
            // пришли. Single-writer соблюдён: MoveTargetPoint больше не пишет никто.
            _moveTargetPoint.Value = GetCurrentTarget();
        }

        private void TickPause(float deltaTime)
        {
            _pauseTimer -= deltaTime;

            if (_pauseTimer > 0f)
            {
                return;
            }

            _pauseTimer = 0f;
            _isPaused = false;

            // Разворот происходит ровно в момент истечения паузы, а не в момент
            // прибытия. Иначе слайм отстаивал бы паузу уже развёрнутым и вид
            // получался бы «дёрнулся, потом подумал».
            _isMovingToB = !_isMovingToB;
        }

        private void TryStartPauseOnArrival()
        {
            if (_rigidbody == null)
            {
                return;
            }

            Vector2 toTarget = GetCurrentTarget() - _rigidbody.position;

            if (toTarget.magnitude > _arriveDistance)
            {
                return;
            }

            _isPaused = true;
            _pauseTimer = _random.Range(_pauseMin, _pauseMax);
        }

        private Vector2 GetCurrentTarget()
        {
            if (_isMovingToB)
            {
                return _pointB;
            }

            return _pointA;
        }

        private float GetSqrDistanceToStart(Vector2 point)
        {
            if (_rigidbody == null)
            {
                return 0f;
            }

            return (point - _rigidbody.position).sqrMagnitude;
        }
    }
}
