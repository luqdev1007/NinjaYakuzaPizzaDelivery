using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GravityFeature
{
    public class GravitySystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private IReadOnlyVariable<float> _baseGravity;
        private IReadOnlyVariable<float> _modifier;
        private IReadOnlyVariable<Vector2> _direction;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _baseGravity = entity.BaseGravity;
            _modifier = entity.GravityModifier;
            _direction = entity.GravityDirection;
        }

        public void OnUpdate(float deltaTime)
        {
            // Итоговая сила гравитации
            float finalScale = _baseGravity.Value * _modifier.Value;

            // Если гравитация стандартная (вниз)
            if (_direction.Value == Vector2.down)
            {
                _rigidbody.gravityScale = finalScale;
            }
            else
            {
                // Если ось изменена (например, на X), выключаем стандартную гравитацию
                // и прикладываем силу вручную (для зон с потоками)
                _rigidbody.gravityScale = 0;
                _rigidbody.AddForce(_direction.Value * (finalScale * 9.81f * _rigidbody.mass));
            }
        }
    }
}