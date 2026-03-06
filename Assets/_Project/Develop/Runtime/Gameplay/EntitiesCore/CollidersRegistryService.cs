using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class CollidersRegistryService
    {
        private Dictionary<Collider2D, Entity> _colliderToEntity = new();

        public void Register(Collider2D collider, Entity entity)
        {
            _colliderToEntity.Add(collider, entity);
        }

        public void Unregister(Collider2D collider)
        {
            _colliderToEntity.Remove(collider);
        }

        public Entity GetBy(Collider2D collider)
        {
            if (_colliderToEntity.TryGetValue(collider, out Entity entity))
                return entity;

            return null;
        }
    }
}
