using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesHelper
    {
        public static bool TryTakeDamageFrom(Entity source, Entity damageable, float damage)
        {
            if (damageable.TryGetTakeDamageRequest(out ReactiveEvent<float> takeDamageRequest) == false)
                return false;
     
            if (IsSameTeam(source, damageable)) 
                return false;

            takeDamageRequest.Invoke(damage);

            return true;
        }

        public static bool IsSameTeam(Entity firstEntity, Entity secondEntity)
        {
            if (firstEntity.TryGetTeam(out ReactiveVariable<Teams> sourceTeam)
                && secondEntity.TryGetTeam(out ReactiveVariable<Teams> targetTeam))
            {
                return sourceTeam.Value == targetTeam.Value;
            }

            return false;
        }
    }
}
