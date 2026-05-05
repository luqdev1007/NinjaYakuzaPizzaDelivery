using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WindFeature
{
    public class WindSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private ReactiveVariable<bool> _isGliding;

        private readonly List<WindArea> _activeZones = new List<WindArea>();

        public void OnInit(Entity entity)
        {
            _rigidbody = entity.Rigidbody;
            _isGliding = entity.IsGliding;

            var triggerProxy = entity.Transform.GetComponent<EntityCollisionsProxy>();

            if (triggerProxy != null)
            {
                triggerProxy.OnTriggerEntered += HandleTriggerEnter;
                triggerProxy.OnTriggerExited += HandleTriggerExit;
            }
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_activeZones.Count == 0) 
                return;

            for (int i = _activeZones.Count - 1; i >= 0; i--)
            {
                var zone = _activeZones[i];

                if (zone == null || !zone.gameObject.activeInHierarchy)
                {
                    _activeZones.RemoveAt(i);
                    continue;
                }

                ApplyWind(zone);
            }
        }

        private void ApplyWind(WindArea zone)
        {
            if (zone.OnlyForGliding && !_isGliding.Value) 
                return;

            Vector2 force = zone.WindForce;

            if (_isGliding.Value)
            {
                force *= zone.GlideMultiplier;
            }

            _rigidbody.AddForce(force);
        }

        private void HandleTriggerEnter(Collider2D other)
        {
            if (other.TryGetComponent(out WindArea zone))
            {
                if (!_activeZones.Contains(zone))
                    _activeZones.Add(zone);
            }
        }

        private void HandleTriggerExit(Collider2D other)
        {
            if (other.TryGetComponent(out WindArea zone))
            {
                _activeZones.Remove(zone);
            }
        }
    }
}