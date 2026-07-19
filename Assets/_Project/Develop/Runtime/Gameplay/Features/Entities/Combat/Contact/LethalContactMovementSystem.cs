using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Contact
{
    public class LethalContactMovementSystem : IInitializableSystem, IFixedUpdatableSystem
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

        // «Скорость = урон» читает velocity на fixed-канале: система зарегистрирована
        // после Dash и Plunge, поэтому в этом же тике видит скорость, которую они
        // записали, а не промежуточное значение между физ-шагами (так было на Update).
        //
        // Контактный буфер по-прежнему наполняют BodyContactDetecting/Filter на Update:
        // они общие для 6 типов сущностей, и их перенос ломает снаряды (см. R7).
        // При FPS >= 50 буфер фактически свежий (m_AutoSyncTransforms: 0 — между
        // физ-шагами OverlapCollider отдаёт то же самое), ниже 50 частота детекта
        // остаётся равной Update-частоте, как и до переноса.
        public void OnFixedUpdate(float deltaTime)
        {
            float currentSpeed = _rigidbody.linearVelocity.magnitude;
            float calculatedDamage = CalculateDamageBySpeed(currentSpeed);

            for (int i = 0; i < _contacts.Count; i++)
            {
                Entity contactEntity = _contacts.Items[i];

                if (_processedEntities.Contains(contactEntity) == false)
                {
                    // СЕМАНТИКА СПИСКА: «уже получил урон за эту серию контакта»,
                    // а НЕ «уже просмотрен». Отсюда и положение Add — ВНУТРИ
                    // гейта урона, вместе с самим уроном.
                    //
                    // Раньше Add стоял снаружи, и список означал «просмотрен».
                    // Это молча блокировало урон в сценарии, где контакт возник
                    // ВНЕ дэша, а дэш начался позже БЕЗ РАЗРЫВА контакта: цель
                    // уже лежала в списке, ветка урона не исполнялась никогда.
                    // Пока герой пролетал сквозь цель за один-два тика, баг был
                    // не виден — контакт рвался, запись вычищалась нижним циклом.
                    // Проявился он на языке слайма, который держит героя в
                    // непрерывном контакте секундами.
                    //
                    // Дедупликация при этом НЕ ослаблена: пока контакт не
                    // разорван, повторный урон по той же цели невозможен —
                    // после первого попадания запись в списке уже есть.
                    if (calculatedDamage > 0f && (_isPlunging.Value == true || _isDashing.Value == true))
                    {
                        _processedEntities.Add(contactEntity);

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
