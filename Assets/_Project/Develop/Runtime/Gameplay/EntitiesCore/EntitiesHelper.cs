using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesHelper
    {
        public static bool TryTakeDamageFrom(Entity source, Entity damageable, float damage)
        {
            if (damageable.TryGetTakeDamageRequest(out ReactiveEvent<DamageData> takeDamageRequest) == false)
                return false;

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

            return true;
        }

        /// <summary>
        /// Отправляет запрос на урон цели, НАМЕРЕННО ОБХОДЯ тим-фильтр: команды
        /// источника и цели не сравниваются вообще, поэтому урон долетит и до
        /// союзников источника, и до нейтральных сущностей без компонента Team.
        /// </summary>
        /// <remarks>
        /// Сценарий, ради которого метод существует — ПЛОЩАДНОЙ УРОН, задевающий
        /// всех в радиусе. Взрыв призрака-камикадзе не разбирает, кто попал в
        /// эпицентр: игрок, другой враг той же команды или разрушаемый проп.
        /// Проверка команд здесь была бы не фильтром, а багом — она молча
        /// выключила бы половину поражаемых целей.
        ///
        /// Отличие от <see cref="TryTakeDamageFrom"/>, помимо тим-фильтра:
        /// DamageData передаётся ЦЕЛИКОМ, как её собрал вызывающий, включая
        /// KnockbackForce и Type. TryTakeDamageFrom собирает DamageData сам и
        /// заполняет только Amount и SourcePosition, из-за чего отбрасывание по
        /// его пути никогда не срабатывает (DamageKnockbackSystem выходит на
        /// нулевой магнитуде).
        ///
        /// НЕ использовать для направленных атак по конкретной цели — там нужен
        /// тим-фильтр, то есть TryTakeDamageFrom.
        ///
        /// Гейт canApplyDamage цели этот метод не обходит: его по-прежнему
        /// проверяет ApplyDamageSystem на стороне получателя.
        /// </remarks>
        /// <returns>true, если запрос был отправлен; false, если у цели нет
        /// компонента TakeDamageRequest.</returns>
        public static bool TryTakeDamageIgnoringTeams(Entity source, Entity damageable, DamageData damageData)
        {
            if (damageable.TryGetTakeDamageRequest(out ReactiveEvent<DamageData> takeDamageRequest) == false)
                return false;

            takeDamageRequest.Invoke(damageData);

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

        public static bool AreOpponents(Entity a, Entity b)
        {
            if (a.TryGetTeam(out ReactiveVariable<Teams> aTeam) == false)
                return false;

            if (b.TryGetTeam(out ReactiveVariable<Teams> bTeam) == false)
                return false;

            return aTeam.Value != bTeam.Value;
        }
    }
}
