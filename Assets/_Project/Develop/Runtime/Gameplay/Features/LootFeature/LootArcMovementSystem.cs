using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootArcMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private const float TravelTime = 1.0f; 
        private const float ArcHeight = 2.5f;  

        private Transform _transform;
        private ReactiveVariable<Entity> _currentTarget;
        private ICompositeCondition _canMove;

        private float _elapsedTime;
        private Vector3 _startPosition;

        private readonly float _travelTime;
        private readonly float _arcHeight;

        public LootArcMovementSystem(float travelTime, float arcHeight)
        {
            _travelTime = travelTime;
            _arcHeight = arcHeight;
        }

        public void OnInit(Entity entity)
        {
            /*
            _transform = entity.Transform;
            _currentTarget = entity.CurrentTarget;
            _canMove = entity.CanMove;
            */
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canMove.Evaluate() == false || _currentTarget.Value == null)
            {
                _elapsedTime = 0;
                return;
            }

            if (_elapsedTime == 0)
            {
                _startPosition = _transform.position;

                // ВАЖНО: Выключаем физику, чтобы она не дергала объект во время Lerp
                if (_transform.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.simulated = false;
                }
            }

            _elapsedTime += deltaTime;

            // Вычисляем прогресс полета (0..1)
            float t = Mathf.Clamp01(_elapsedTime / TravelTime);

            // Плавное ускорение (Ease In)
            float easeT = t * t * t;

            // 1. Базовая позиция (движение по прямой линии к цели)
            Vector3 lerpPosition = Vector3.zero; // Vector3.Lerp(_startPosition, _currentTarget.Value.Transform.position, easeT);

            // 2. Вычисляем "горб" дуги с помощью синуса
            // Sin(0) = 0, Sin(PI/2) = 1 (пик), Sin(PI) = 0
            float arc = Mathf.Sin(t * Mathf.PI) * ArcHeight;

            // 3. Складываем: прямая линия + смещение вверх по дуге
            _transform.position = lerpPosition + new Vector3(0, arc, 0);
        }
    }
}