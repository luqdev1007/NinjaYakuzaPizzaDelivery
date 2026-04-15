using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WindFeature
{
    public class WindSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private ReactiveVariable<bool> _isGliding;
        private Transform _transform;

        // Список зон, в которых мы сейчас находимся
        private readonly List<WindArea> _activeZones = new List<WindArea>();
        private ContactFilter2D _filter;

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGliding = entity.IsGliding;
            _transform = entity.Transform;

            _filter = new ContactFilter2D();
            _filter.useTriggers = true;
        }

        private void UpdateActiveZones()
        {
            _activeZones.Clear();
            Collider2D[] results = new Collider2D[5];
            int count = Physics2D.OverlapCollider(_rigidbody.GetComponent<Collider2D>(), _filter, results);

            for (int i = 0; i < count; i++)
            {
                if (results[i].TryGetComponent(out WindArea zone))
                {
                    _activeZones.Add(zone);
                }
            }
        }

        private void ApplyWind(WindArea zone, float deltaTime)
        {
            if (zone.OnlyForGliding && !_isGliding.Value) return;

            Vector2 force = zone.WindForce;

            // Если парашют открыт, ветер «парусит» и толкает сильнее
            if (_isGliding.Value)
            {
                force *= zone.GlideMultiplier;
            }

            // Применяем силу (ForceMode2D.Force для плавного влияния ветра)
            _rigidbody.AddForce(force);
        }

        public void OnUpdate(float deltaTime)
        {
            UpdateActiveZones();

            foreach (var zone in _activeZones)
            {
                ApplyWind(zone, deltaTime);
            }
        }
    }
}