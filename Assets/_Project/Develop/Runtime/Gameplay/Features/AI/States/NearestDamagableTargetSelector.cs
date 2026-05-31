using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
// using Assets._Project.Develop.Runtime.Gameplay.Common; // Для EntitiesHelper
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class NearestDamagableTargetSelector : ITargetSelector
    {
        private readonly Entity _source;
        private readonly Transform _sourceTransform;

        public NearestDamagableTargetSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Transform; 
        }

        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            Entity closestTarget = null;
            float minSqrDistance = float.MaxValue;

            foreach (Entity target in targets)
            {
                if (target == _source) continue;

                if (!target.HasComponent<TakeDamageRequest>()) continue;

                if (EntitiesHelper.IsSameTeam(_source, target)) continue;

                if (target.TryGetCanApplyDamage(out ICompositeCondition canApplyDamage) 
                    && !canApplyDamage.Evaluate()) continue; 

                float sqrDistance = GetSqrDistanceTo(target);

                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        private float GetSqrDistanceTo(Entity target)
        {
            Vector2 offset = target.Transform.position - _sourceTransform.position;
            return offset.sqrMagnitude;
        }
    }
}