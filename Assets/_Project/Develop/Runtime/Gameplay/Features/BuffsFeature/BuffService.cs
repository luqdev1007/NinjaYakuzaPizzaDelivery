using Assets._Project.Develop.Runtime.Configs.Gameplay.Buffs;
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
                existingBuff.Extend(config.Duration); 

                return;
            }

            IBuffEffect effect = config.CreateEffect();
            effect.Apply(hero);

            ActiveBuff newBuff = new ActiveBuff(config.Id, effect, config.Duration, config.Icon); 
            activeBuffs.Add(newBuff);
        }
    }
}