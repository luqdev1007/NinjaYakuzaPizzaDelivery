using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackCancelSystem : IInitializableSystem, IUpdatableSystem
    {
        private ReactiveVariable<bool> _inAttackProcess;
        private ReactiveEvent _attackCanceledEvent;
        private ICompositeCondition _mustCancelAttack;

        public void OnInit(Entity entity)
        {
            _inAttackProcess = entity.InAttackProcess;
            _attackCanceledEvent = entity.AttackCanceledEvent;
            _mustCancelAttack = entity.MustCancelAttack;
        }

        public void OnUpdate(float deltaTime)
        {
            if (!_inAttackProcess.Value)
                return;

            if (_mustCancelAttack.Evaluate())
            {
                Cancel();
            }
        }

        private void Cancel()
        {
            _inAttackProcess.Value = false;
            _attackCanceledEvent.Invoke();

            Debug.Log("Attack Process Canceled");
        }
    }
}