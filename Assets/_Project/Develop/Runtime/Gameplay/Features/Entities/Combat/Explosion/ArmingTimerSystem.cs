using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Explosion
{
    /// <summary>
    /// Тикает окно взведения. Пока IsArming — уменьшает ArmingTimer, на нуле шлёт
    /// DetonationRequest с DetonationKind.Natural.
    /// </summary>
    /// <remarks>
    /// Сброс таймера — НЕ ответственность этой системы: значение выставляет
    /// ArmingState.Enter, флаг снимает ArmingState.Exit. Здесь только тик, поэтому
    /// при IsArming == false система не делает ничего и таймер не трогает —
    /// иначе получилось бы два писателя одного поля.
    ///
    /// Канал fixed — по образцу остальных боевых таймеров (ApplyDamageCooldownSystem,
    /// AttackInvulnerabilitySystem): окно взведения соревнуется с движением игрока,
    /// а движение живёт на fixed.
    /// </remarks>
    public class ArmingTimerSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private ReactiveVariable<bool> _isArming;
        private ReactiveVariable<float> _armingTimer;
        private ReactiveEvent<DetonationKind> _detonationRequest;

        public void OnInit(Entity entity)
        {
            _isArming = entity.IsArming;
            _armingTimer = entity.ArmingTimer;
            _detonationRequest = entity.DetonationRequest;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_isArming.Value == false)
            {
                return;
            }

            if (_armingTimer.Value <= 0f)
            {
                return;
            }

            _armingTimer.Value -= deltaTime;

            if (_armingTimer.Value <= 0f)
            {
                _detonationRequest.Invoke(DetonationKind.Natural);
            }
        }
    }
}
