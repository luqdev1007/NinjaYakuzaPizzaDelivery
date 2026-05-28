using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Contact
{
    public class LethalContactMovementSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private Buffer<Entity> _contacts;
        private Rigidbody2D _rigidbody;

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

            _processedEntities = new List<Entity>(_contacts.Items.Length);

            Debug.Log($"<color=white>[LethalContact]</color> Система инициализирована для {_entity}.");
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

                    if (calculatedDamage > 0f)
                    {
                        EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, calculatedDamage);

                        // Зеленый лог — успешный смертоносный влет
                        Debug.Log($"<color=green>[LethalContact] НАНОСИМ УРОН!</color> Сущность {_entity} врезалась в {contactEntity}. " +
                                  $"Скорость: <b>{currentSpeed:F2}</b>, Урон: <b>{calculatedDamage}</b>");
                    }
                    else
                    {
                        // Желтый лог — коснулись, но скорость слишком мала для урона
                        Debug.Log($"<color=yellow>[LethalContact] Мирный контакт.</color> Тормозим или просто тремся об {contactEntity}. " +
                                  $"Скорость: <b>{currentSpeed:F2}</b> (нужно минимум {MinSpeedThreshold})");
                    }
                }
            }

            for (int i = _processedEntities.Count - 1; i >= 0; i--)
            {
                Entity processedEntity = _processedEntities[i];
                if (ContainInContacts(processedEntity) == false)
                {
                    // Голубой лог — враг вышел из нашего коллайдера, теперь ему снова можно нанести урон следующим рывком
                    Debug.Log($"<color=cyan>[LethalContact] Контакт разорван.</color> {processedEntity} вышел из зоны поражения. Удаляем из истории.");
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