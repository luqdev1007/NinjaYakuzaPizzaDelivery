using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class AttackProcessTimerSystem : IInitializableSystem, IDisposableSystem, IFixedUpdatableSystem
    {
        private ReactiveVariable<float> _currentTime;
        private ReactiveVariable<bool> _inAttackProcess;
        private ReactiveEvent _startAttackEvent;

        private IDisposable _startAttackDisposable;

        public void OnInit(Entity entity)
        {
            _currentTime = entity.AttackProcessCurrentTime;
            _inAttackProcess = entity.InAttackProcess;
            _startAttackEvent = entity.StartAttackEvent;

            _startAttackDisposable = _startAttackEvent.Subscribe(OnStartAttackProcess);
        }

        // Единственный драйвер окна атаки. Остальная цепочка тикового канала не имеет
        // и висит на подписке на _currentTime: AttackDelayEndTriggerSystem -> хитбокс
        // MeleeAttackHitSystem -> SuccessfulHitEvent -> i-frames/хитстоп. Поэтому
        // перевод этого таймера на fixed тянет всю цепочку в тот же физ-тик —
        // окно атаки и OverlapCircleAll совпадают с физикой контактов.
        public void OnFixedUpdate(float deltaTime)
        {
            if (_inAttackProcess.Value)
            {
                _currentTime.Value += deltaTime;
            }
        }

        private void OnStartAttackProcess()
        {
            _currentTime.Value = 0f;
        }

        public void OnDispose()
        {
            _startAttackDisposable?.Dispose();
        }
    }
}
