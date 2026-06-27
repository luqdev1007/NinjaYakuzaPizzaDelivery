using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class BuffsTimerSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private ActiveBuffsList _activeBuffs;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _activeBuffs = entity.ActiveBuffs;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_activeBuffs.Elements.Count == 0)
            {
                return;
            }

            List<ActiveBuff> expiredBuffs = null;

            foreach (ActiveBuff activeBuff in _activeBuffs.Elements)
            {
                activeBuff.RemainingTime.Value -= deltaTime;

                if (activeBuff.RemainingTime.Value <= 0f)
                {
                    if (expiredBuffs == null)
                    {
                        expiredBuffs = new List<ActiveBuff>();
                    }

                    expiredBuffs.Add(activeBuff);
                }
            }

            if (expiredBuffs == null)
            {
                return;
            }

            foreach (ActiveBuff expiredBuff in expiredBuffs)
            {
                expiredBuff.Effect.Remove(_entity);
                _activeBuffs.Remove(expiredBuff);
            }
        }
    }
}