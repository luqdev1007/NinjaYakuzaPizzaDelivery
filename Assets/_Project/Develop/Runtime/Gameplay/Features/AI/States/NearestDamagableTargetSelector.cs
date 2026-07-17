using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
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

        public Entity SelectTargetFrom(IEnumerable<Entity> targets, Entity excluded, float sqrRadius)
        {
            Entity closestTarget = null;
            float minSqrDistance = float.MaxValue;

            foreach (Entity target in targets)
            {
                float sqrDistance = GetSqrDistanceTo(target);

                if (IsValidTarget(target, excluded, sqrDistance, sqrRadius) == false)
                    continue;

                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        private bool IsValidTarget(Entity target, Entity excluded, float sqrDistance, float sqrRadius)
        {
            if (target == _source)
                return false;

            if (target == excluded)
                return false;

            if (target.HasComponent<TakeDamageRequest>() == false)
                return false;

            if (EntitiesHelper.AreOpponents(_source, target) == false)
                return false;

            if (target.TryGetCanApplyDamage(out ICompositeCondition canApplyDamage)
                && canApplyDamage.Evaluate() == false)
                return false;

            if (sqrDistance > sqrRadius)
                return false;

            return true;
        }

        private float GetSqrDistanceTo(Entity target)
        {
            if (target == null || target.Transform == null)
                return float.MaxValue;

            Vector2 offset = target.Transform.position - _sourceTransform.position;
            return offset.sqrMagnitude;
        }
    }
}
