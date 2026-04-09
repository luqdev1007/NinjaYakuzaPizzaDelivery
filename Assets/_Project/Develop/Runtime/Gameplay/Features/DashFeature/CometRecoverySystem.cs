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

            // 1. Если таймер активен — уменьшаем его
            if (state.CooldownTimer.Value > 0)
            {
                state.CooldownTimer.Value -= deltaTime;
                return;
            }

            // 2. Если время вышло, начинаем восстановление
            // Восстанавливаем множитель до 1.0 (можно сделать плавно через deltaTime)
            if (state.CurrentMultiplier.Value < 1f)
            {
                // Либо мгновенно: state.CurrentMultiplier.Value = 1f;
                // Либо плавно (например, за 1 секунду):
                state.CurrentMultiplier.Value = Mathf.MoveTowards(state.CurrentMultiplier.Value, 1f, deltaTime);
            }

            // 3. Восстанавливаем заряды рывка (Comet Dash)
            if (state.CurrentCharges.Value < state.Config.MaxCharges)
            {
                state.CurrentCharges.Value = state.Config.MaxCharges;
                Debug.Log("<color=green>[COMET]</color> Charges Restored");
            }
        }
    }
}