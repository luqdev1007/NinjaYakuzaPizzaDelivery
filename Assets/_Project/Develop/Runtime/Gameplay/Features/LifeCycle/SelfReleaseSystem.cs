using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    public class SelfReleaseSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private Entity _entity;

        public SelfReleaseSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.Transform == null)
                return;

            if (_entity.MustSelfRelease.Evaluate())
            {
                ExecuteRelease();
            }
        }

        private void ExecuteRelease()
        {
            _entitiesLifeContext.Release(_entity);
        }
    }
}