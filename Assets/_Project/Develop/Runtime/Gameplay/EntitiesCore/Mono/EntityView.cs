using UnityEngine;
using Assets._Project.Develop.Infrastructure.DI;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public abstract class EntityView : MonoBehaviour
    {
        public void ResolveDependencies(DIContainer container)
        {
            OnDependencyResolve(container);
        }

        protected virtual void OnDependencyResolve(DIContainer container)
        {
        }

        public void Link(Entity entity)
        {
            entity.Initialized += OnEntityStartedWork;
        }

        public virtual void Cleanup(Entity entity)
        {
            entity.Initialized -= OnEntityStartedWork;
        }

        protected abstract void OnEntityStartedWork(Entity entity);
    }
}