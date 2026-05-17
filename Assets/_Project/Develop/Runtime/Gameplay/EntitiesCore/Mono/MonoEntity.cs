using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class MonoEntity : MonoBehaviour
    {
        private CollidersRegistryService _collidersRegistryService;
        private Entity _linkedEntity;

        private IAudioService _audioService;

        public Entity LinkedEntity => _linkedEntity;

        public void Initialize(CollidersRegistryService collidersRegistryService, IAudioService audioService)
        {
            _collidersRegistryService = collidersRegistryService;
            _audioService = audioService; 
        }

        public void Link(Entity entity)
        {
            _linkedEntity = entity;

            EntityView[] views = GetComponentsInChildren<EntityView>();

            MonoEntityRegistrator[] registrators = GetComponentsInChildren<MonoEntityRegistrator>();

            if (registrators != null)
                foreach (MonoEntityRegistrator registrator in registrators)
                    registrator.Register(entity);

            if (views != null)
            {
                foreach (EntityView entityView in views)
                {
                    if (entityView is IRequireAudioService audioView)
                        audioView.Construct(_audioService);

                    entityView.Link(entity);
                }
            }

            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
                _collidersRegistryService.Register(collider, entity);
        }

        public void Cleanup(Entity entity)
        {
            if (this == null) 
                return;

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
