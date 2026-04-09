using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class CometRecoverySystem : IUpdatableSystem
    {
        private Entity _entity;

        public void OnInit(Entity entity) => _entity = entity;

        public void OnUpdate(float deltaTime)
        {
            CometDashStateComponent state = _entity.CometDashStateC; ; // _entity.CometDashState;

            // 1. Уменьшаем таймер кулдауна
            if (state.CooldownTimer.Value > 0)
            {
                state.CooldownTimer.Value -= deltaTime;
                return; // Пока висит КД, заряды не регеним
            }

            // 2. Если кулдаун вышел, а зарядов меньше макса — регеним по одному
            if (state.CurrentCharges.Value < state.MaxCharges)
            {
                state.CurrentCharges.Value = state.MaxCharges;

                // Сбрасываем множитель в исходное состояние (1.0)
                state.CurrentMultiplier.Value = 1f;

                Debug.Log("<color=green>[COMET]</color> Charges Restored!");
            }
        }
    }
}