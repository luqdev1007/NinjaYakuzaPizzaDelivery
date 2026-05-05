using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class CometRecoverySystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _entity;
        private ReactiveVariable<bool> _isRecovering;

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _isRecovering = entity.IsCometRecovering;
        }

        public void OnUpdate(float deltaTime)
        {
            var state = _entity.CometDashStateC;
            bool isFull = state.CurrentCharges.Value >= state.Config.MaxCharges;

            if (isFull)
            {
                if (_isRecovering.Value) _isRecovering.Value = false;
                return;
            }

            if (state.CooldownTimer.Value > 0)
            {
                state.CooldownTimer.Value -= deltaTime;
                _isRecovering.Value = true;
                return;
            }

            state.CurrentCharges.Value++;

            Debug.Log($"<color=cyan>[COMET]</color> Charge Restored: {state.CurrentCharges.Value}/{state.Config.MaxCharges}");

            if (state.CurrentCharges.Value >= state.Config.MaxCharges)
            {
                state.CurrentMultiplier.Value = 1f;
                state.CooldownTimer.Value = 0f;
                _isRecovering.Value = false;
                Debug.Log("<color=green>[COMET]</color> Resources Fully Recovered");
            }
            else
            {
                state.CooldownTimer.Value = state.Config.BaseCooldown;
                state.CurrentMultiplier.Value = Mathf.Min(1f, state.CurrentMultiplier.Value + 0.2f);
            }
        }
    }
}