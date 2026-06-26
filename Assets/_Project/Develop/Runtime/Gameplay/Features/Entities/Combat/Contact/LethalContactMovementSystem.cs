using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Contact
{
    public class LethalContactMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private Buffer<Entity> _contacts;
        private Rigidbody2D _rigidbody;

        private ReactiveVariable<bool> _isPlunging;
        private ReactiveVariable<bool> _isDashing;
        private ReactiveEvent _speedDamageDealtEvent;

        private List<Entity> _processedEntities;

        private const float MinSpeedThreshold = 10f;
        private const float MaxSpeedThreshold = 20f;
        private const float BaseLethalDamage = 10f;
        private const float MaxLethalDamage = 20f;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _contacts = entity.ContactEntitiesBuffer;
            _rigidbody = entity.Rigidbody;

            _isPlunging = entity.IsPlunging;
            _isDashing = entity.IsDashing;
            _speedDamageDealtEvent = entity.SpeedDamageDealtEvent;

            _processedEntities = new List<Entity>(_contacts.Items.Length);
        }

        public void OnUpdate(float deltaTime)
        {
            float currentSpeed = _rigidbody.linearVelocity.magnitude;
            float calculatedDamage = CalculateDamageBySpeed(currentSpeed);

            for (int i = 0; i < _contacts.Count; i++)
            {
                Entity contactEntity = _contacts.Items[i];

                if (_processedEntities.Contains(contactEntity) == false)
                {
                    _processedEntities.Add(contactEntity);

                    if (calculatedDamage > 0f && (_isPlunging.Value == true || _isDashing.Value == true))
                    {
                        if (EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, calculatedDamage))
                            _speedDamageDealtEvent?.Invoke();
                    }
                }
            }

            for (int i = _processedEntities.Count - 1; i >= 0; i--)
            {
                Entity processedEntity = _processedEntities[i];

                if (ContainInContacts(processedEntity) == false)
                {
                    _processedEntities.RemoveAt(i);
                }
            }
        }

        private float CalculateDamageBySpeed(float speed)
        {
            if (speed < MinSpeedThreshold)
                return 0f;

            if (speed >= MaxSpeedThreshold)
                return MaxLethalDamage;

            if (speed >= MinSpeedThreshold)
                return BaseLethalDamage;

            return 0f;
        }

        public bool ContainInContacts(Entity entity)
        {
            for (int i = 0; i < _contacts.Count; i++)
                if (_contacts.Items[i] == entity)
                    return true;

            return false;
        }
    }
}