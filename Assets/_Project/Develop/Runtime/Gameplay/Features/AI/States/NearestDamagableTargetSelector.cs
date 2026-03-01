using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
    public class NearestDamagableTargetSelector : ITargetSelector
    {
        private Entity _source;
        private Transform _sourceTransform;

        public NearestDamagableTargetSelector(Entity entity)
        {
            _source = entity;
            _sourceTransform = entity.Transform;
        }

        public Entity SelectTargetFrom(IEnumerable<Entity> targets)
        {
            if (targets == null) 
                return null;

            Entity closestTarget = null;
            float minDistance = float.MaxValue;

            foreach (Entity target in targets)
            {
                if (target == null || target.Transform == null || target == _source)
                    continue;

                if (target.HasComponent<TakeDamageRequest>() == false)
                    continue;

                if (EntitiesHelper.IsSameTeam(_source, target))
                    continue;

                if (target.TryGetCanApplyDamage(out ICompositeCondition canApplyDamage))
                {
                    if (canApplyDamage.Evaluate() == false)
                        continue;
                }

                Vector3 diff = target.Transform.position - _sourceTransform.position;
                float curSqrDistance = diff.sqrMagnitude;

                if (curSqrDistance < minDistance)
                {
                    minDistance = curSqrDistance;
                    closestTarget = target;
                }
            }

            if (closestTarget == null)
            {
                // Debug.Log("[TargetSelector] Подходящих целей не найдено.");
            }
            else
            {
                // Debug.Log($"[TargetSelector] Найдена цель: {closestTarget.Transform.gameObject.name}");
            }

            return closestTarget;
        }
    }
}
