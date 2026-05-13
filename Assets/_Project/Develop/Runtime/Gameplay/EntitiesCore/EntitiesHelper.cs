using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesHelper
    {
        public static bool TryTakeDamageFrom(Entity source, Entity damageable, float damage)
        {
            /*
            if (damageable.TryGetTakeDamageRequest(out ReactiveEvent<DamageData> takeDamageRequest) == false)
                return false;

            // Проверка команд
            if (source.TryGetTeam(out ReactiveVariable<Teams> sourceTeam)
                && damageable.TryGetTeam(out ReactiveVariable<Teams> damageableTeam))
            {
                if (sourceTeam.Value == damageableTeam.Value)
                    return false;
            }

            DamageData data = new DamageData
            {
                Amount = damage,
                SourcePosition = source.Transform.position 
            };

            takeDamageRequest.Invoke(data);
            */

            return true;
        }

        public static bool IsSameTeam(Entity firstEntity, Entity secondEntity)
        {
            /*
            if (firstEntity.TryGetTeam(out ReactiveVariable<Teams> sourceTeam)
                && secondEntity.TryGetTeam(out ReactiveVariable<Teams> targetTeam))
            {
                return sourceTeam.Value == targetTeam.Value;
            }
            */

            return false;
        }
    }
}