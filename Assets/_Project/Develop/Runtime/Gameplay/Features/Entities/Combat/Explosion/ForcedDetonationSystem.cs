using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.Combat.Explosion
{
    /// <summary>
    /// Подрывает призрака, убитого до конца взведения. Слушает IsDead и при
    /// переходе в true шлёт DetonationRequest с DetonationKind.Forced.
    /// </summary>
    /// <remarks>
    /// Проверка HasDetonated обязательна и не является перестраховкой: штатный
    /// взрыв сам ставит CurrentHealth = 0, то есть тоже приводит к IsDead = true и
    /// доходит сюда. Без флага каждый нормальный взрыв немедленно вызывал бы
    /// второй, вынужденный.
    ///
    /// Регистрировать ДО DisableCollidersOnDeathSystem. Порядок подписки на IsDead
    /// определяется порядком AddSystem, а обе системы висят на одном флаге.
    /// Фактической зависимости нет — взрыв читает чужие коллайдеры через
    /// CollidersRegistryService, а не свои, — но порядок зафиксирован явно, чтобы
    /// поведение не держалось на случайности.
    /// </remarks>
    public class ForcedDetonationSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<bool> _hasDetonated;
        private ReactiveEvent<DetonationKind> _detonationRequest;

        private IDisposable _isDeadDisposable;

        public void OnInit(Entity entity)
        {
            _hasDetonated = entity.HasDetonated;
            _detonationRequest = entity.DetonationRequest;

            _isDeadDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        public void OnDispose()
        {
            _isDeadDisposable?.Dispose();
        }

        private void OnIsDeadChanged(bool oldValue, bool isDead)
        {
            if (isDead == false)
            {
                return;
            }

            if (_hasDetonated.Value)
            {
                return;
            }

            _detonationRequest.Invoke(DetonationKind.Forced);
        }
    }
}
