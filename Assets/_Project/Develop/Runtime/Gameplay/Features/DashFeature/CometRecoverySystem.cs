using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class CometRecoverySystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;

        public void OnInit(Entity entity) => _entity = entity;

        public void OnUpdate(float deltaTime)
        {
            var state = _entity.CometDashStateC;

            // 1. Пока идет кулдаун (Base или Overheat), ресурсы не восстанавливаются
            if (state.CooldownTimer.Value > 0)
            {
                state.CooldownTimer.Value -= deltaTime;
                return;
            }

            // 2. Если кулдаун закончился, проверяем, нужно ли восстановить заряды
            if (state.CurrentCharges.Value < state.Config.MaxCharges)
            {
                state.CurrentCharges.Value = state.Config.MaxCharges;
                // При восстановлении зарядов сбрасываем множитель в 1.0 мгновенно
                state.CurrentMultiplier.Value = 1f;
                Debug.Log("<color=green>[COMET]</color> Resources Fully Recovered");
            }

            // 3. Дополнительная страховка множителя (если заряды полные, но множитель почему-то нет)
            if (state.CurrentCharges.Value == state.Config.MaxCharges && state.CurrentMultiplier.Value < 1f)
            {
                state.CurrentMultiplier.Value = 1f;
            }
        }
    }
}