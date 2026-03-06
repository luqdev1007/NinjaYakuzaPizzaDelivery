using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntity : MonoBehaviour
    {
        private CollidersRegistryService _collidersRegistryService;
        private Entity _linkedEntity;

        public Entity LinkedEntity => _linkedEntity;

        public void Initialize(CollidersRegistryService collidersRegistryService)
        {
            _collidersRegistryService = collidersRegistryService;
        }

        public void Link(Entity entity)
        {
            _linkedEntity = entity;

            MonoEntityRegistrator[] registrators = GetComponentsInChildren<MonoEntityRegistrator>();

            if (registrators != null)
                foreach (MonoEntityRegistrator registrator in registrators)
                    registrator.Register(entity);

            EntityView[] views = GetComponentsInChildren<EntityView>();

            if (views != null)
                foreach (EntityView entityView in views)
                    entityView.Link(entity);

            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
                _collidersRegistryService.Register(collider, entity);
        }

        public void Cleanup(Entity entity)
        {
            EntityView[] views = GetComponentsInChildren<EntityView>();

            if (views != null)
                foreach (EntityView entityView in views)
                    entityView.Cleanup(entity);

            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
                _collidersRegistryService.Unregister(collider);

            _linkedEntity = null;
        }
    }
}
