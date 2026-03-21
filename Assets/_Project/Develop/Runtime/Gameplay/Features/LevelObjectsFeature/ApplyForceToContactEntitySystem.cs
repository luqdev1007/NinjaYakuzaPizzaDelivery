using Assets._Project.Develop.Runtime.Gameplay.Common;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelObjectsFeature
{
    internal class ApplyForceToContactEntitySystem : IInitializableSystem, IUpdatableSystem
    {
        private Buffer<Collider2D> _contacts;
        private Buffer<Entity> _contactsEntities;

        private readonly CollidersRegistryService _colllidersRegistryService;

        private ICompositeCondition _canApplyForce;
        private ReactiveVariable<int> _chargesCount; // another system
        private ReactiveVariable<float> _forcePower;

        public ApplyForceToContactEntitySystem(CollidersRegistryService colllidersRegistryService)
        {
            _colllidersRegistryService = colllidersRegistryService;
        }

        public void OnInit(Entity entity)
        {
            _contacts = entity.ContactCollidersBuffer;
            _contactsEntities = entity.ContactEntitiesBuffer;
            _canApplyForce = entity.CanApplyPhysicsFroce;
            _chargesCount = entity.ApplyingForceCharges;
            _forcePower = entity.ApplyingForcePower;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_canApplyForce.Evaluate() == false)
                return;

            _contactsEntities.Count = 0;

            for (int i = 0; i < _contacts.Count; i++)
            {
                Collider2D collider = _contacts.Items[i];

                Entity contactEntity = _colllidersRegistryService.GetBy(collider);

                if (contactEntity != null)
                {
                    if (contactEntity.HasComponent<RigidbodyComponent>())
                    {
                        contactEntity.Rigidbody.AddForce(Vector2.up * _forcePower.Value, ForceMode2D.Impulse);
                        _chargesCount.Value--;
                        Debug.Log($"{_chargesCount.Value}"); // ограничения
                    }
                }
            }
        }
    }
}
