using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class BuffService
    {
        public void Pickup(Entity hero, BuffConfig config)
        {
            ActiveBuffsList activeBuffs = hero.ActiveBuffsC.Value;

            if (activeBuffs.TryGetById(config.Id, out ActiveBuff existingBuff))
            {
                existingBuff.RemainingTime.Value += config.Duration;

                return;
            }

            IBuffEffect effect = config.CreateEffect();
            effect.Apply(hero);

            ActiveBuff newBuff = new ActiveBuff(config.Id, effect, config.Duration);
            activeBuffs.Add(newBuff);
        }
    }
}