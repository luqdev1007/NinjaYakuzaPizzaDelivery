using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature
{
    public class RigidbodyMoveTowardsTargetSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ICompositeCondition _canMove;
        private Rigidbody _rigidbody;

        private ITargetSelector _targetSelector;
        private EntitiesLifeContext _entitiesLifeContext;
        private ReactiveVariable<Entity> _target;

        private Entity _lastLoggedTarget; // Для контроля логов

        public RigidbodyMoveTowardsTargetSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _target = entity.CurrentTarget;
            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _canMove = entity.CanMove;
            _rigidbody = entity.Rigidbody;

            _targetSelector = new NearestDamagableTargetSelector(entity);
        }

        public void OnUpdate(float deltaTime)
        {
            // 1. Поиск цели
            if (_target.Value == null)
            {
                _target.Value = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);
                _moveDirection.Value = Vector3.zero;
                return;
            }

            // Логирование при смене цели
            if (_target.Value != _lastLoggedTarget)
            {
                // Debug.Log($"<color=yellow>[MovementSystem]</color> Entity follows: <b>{_target.Value.Transform.name}</b>");
                _lastLoggedTarget = _target.Value;
            }

            if (_rigidbody == null || _canMove.Evaluate() == false)
                return;

            // 2. Расчет направления и позиций (Фикс оси Y)
            Vector3 currentPos = _rigidbody.position;
            Vector3 targetPos = _target.Value.Transform.position;

            // Зануляем разницу по высоте для расчета движения и поворота
            Vector3 flatTargetPos = new Vector3(targetPos.x, currentPos.y, targetPos.z);
            Vector3 directionToTarget = (flatTargetPos - currentPos).normalized;

            float distance = Vector3.Distance(currentPos, flatTargetPos);

            // Если мы ближе чем на 0.1 единицу, перестаем двигаться
            if (distance > 0.1f)
            {
                Vector3 nextStep = Vector3.MoveTowards(currentPos, flatTargetPos, _moveSpeed.Value * deltaTime);
                _rigidbody.MovePosition(nextStep);
            }
            else
            {
                // Сбрасываем скорость, чтобы физика не накапливала импульс
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }

            // 4. Физический поворот (Лицом к цели)
            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                _rigidbody.MoveRotation(targetRotation);

                // Обновляем реактивное направление для внешних систем (например, анимаций)
                _moveDirection.Value = directionToTarget;
            }
        }
    }
}