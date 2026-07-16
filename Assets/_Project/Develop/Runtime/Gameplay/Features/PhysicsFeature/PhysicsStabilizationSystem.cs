using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PhysicsFeature
{
    // Гасит linearVelocity, оставшуюся от арки knockback, чтобы она красиво оседала.
    // Ротацией больше не занимается — её держит констрейнт FreezeRotation на
    // Rigidbody2D префаба.
    //
    // Канал: fixed. Раньше система жила на Update, тогда как оба других писателя
    // linearVelocity (SimpleRigidbodyMovementSystem, LethalContactMovementSystem)
    // уже на fixed — получались два писателя одного поля на разных каналах.
    public class PhysicsStabilizationSystem : IEntitySystem, IInitializableSystem, IFixedUpdatableSystem
    {
        // Порог в единицах скорости (units/sec), а не в квадрате. Раньше это поле
        // сравнивалось с sqrMagnitude, из-за чего фактический порог был sqrt(0.03)
        // ~= 0.173 — имя врало почти в 6 раз. Сравнение осталось на sqrMagnitude
        // (без корня), но порог возводится в квадрат явно, поэтому значение поля
        // теперь означает ровно то, что написано.
        private readonly float _stabilizationSpeedThreshold = 0.03f;

        private float _linearDrag;
        private Rigidbody2D _linkedRigidbody;
        private ICompositeCondition _canAffect;

        public void OnInit(Entity entity)
        {
            _linearDrag = entity.LinearDrag.Value;
            _canAffect = entity.CanPhysicalyInteract;
            _linkedRigidbody = entity.Rigidbody;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_canAffect.Evaluate() == false)
            {
                return;
            }

            if (_linkedRigidbody.linearVelocity.sqrMagnitude > 0)
            {
                _linkedRigidbody.linearVelocity = Vector2.MoveTowards(
                    _linkedRigidbody.linearVelocity,
                    Vector2.zero,
                    _linearDrag * deltaTime);

                if (_linkedRigidbody.linearVelocity.sqrMagnitude < _stabilizationSpeedThreshold * _stabilizationSpeedThreshold)
                {
                    _linkedRigidbody.linearVelocity = Vector2.zero;
                }
            }
        }
    }
}
