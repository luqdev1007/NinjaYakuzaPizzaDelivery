using Assets._Project.Develop.Runtime.Gameplay.Common;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    public class ReleaseChildEntitiesSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private Entity _entity;

        private ICompositeCondition _mustDie;

        public ReleaseChildEntitiesSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _mustDie = entity.MustDie;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_mustDie.Evaluate())
            {
                TransformEntityRegistrator parentRegistrator = _entity.Transform.GetComponent<TransformEntityRegistrator>();

                TransformEntityRegistrator[] children = _entity.Transform.GetComponentsInChildren<TransformEntityRegistrator>();

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                    if (child == null || child == parentRegistrator)
                        continue;

                    if (child.Instance != null)
                    {
                        _entitiesLifeContext.Release(child.Instance);
                    }
                }
            }
        }
    }
}
