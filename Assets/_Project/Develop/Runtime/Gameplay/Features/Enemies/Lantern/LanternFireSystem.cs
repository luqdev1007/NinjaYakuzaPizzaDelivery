using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Ритмичная стрельба фонаря: метроном без прицеливания. По приватному
    /// таймеру взводит телеграф, по концу телеграфа спавнит снаряд в
    /// ФИКСИРОВАННОМ направлении и уходит на кулдаун.
    ///
    /// СТРЕЛЬБА БЕЗУСЛОВНА: ни sight-check, ни дистанции, ни поиска героя — в
    /// отличие от TongueSystem слайма. Фонарь плюётся огнём по ритму независимо
    /// от того, где игрок. Это осознанно: скилл-чек не в «увернись от прицела», а
    /// в «прочитай ритм и линию огня».
    ///
    /// ТАЙМЕРЫ — приватные float на fixed-канале (правило проекта: игровой ритм
    /// НЕ на TimerService и НЕ на корутинах). Собственный enum фазы по образцу
    /// TongueSystem/GrappleSystem.
    ///
    /// SINGLE-WRITER: система пишет IsTelegraphing на СВОЕЙ сущности. Тот же общий
    /// флаг и та же TelegraphView, что у языка слайма, — но писатель на ЭТОЙ
    /// сущности ровно один (см. шапку IsTelegraphing про per-entity инвариант).
    ///
    /// ГЕЙТ СМЕРТИ. Пока фонарь в death-process (или уже мёртв), не стреляет и
    /// гасит телеграф: труп не должен ни плеваться, ни застыть в сжатом
    /// телеграф-состоянии. Проверка в начале тика.
    ///
    /// НАПРАВЛЕНИЕ И ТОЧКА ВЫЛЕТА приезжают в конструктор из LanternAimAuthoring
    /// (per-instance со сцены), уставки ритма и снаряда — из LanternConfig.
    /// Компонентов под них не заводим: иных читателей нет, а реактивка без
    /// подписчика против правил проекта (то же решение, что по приватным таймерам
    /// TongueSystem).
    /// </summary>
    public class LanternFireSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private enum FireState
        {
            Cooldown,
            Telegraph
        }

        private readonly ProjectileFactory _projectileFactory;

        private readonly float _fireCooldown;
        private readonly float _telegraphDuration;
        private readonly LanternProjectileData _projectileData;
        private readonly Vector2 _shootOrigin;
        private readonly Vector2 _shootDirection;

        private ReactiveVariable<bool> _isTelegraphing;
        private ReactiveVariable<bool> _isDead;
        private ReactiveVariable<bool> _inDeathProcess;

        private FireState _state;
        private float _cooldownTimer;
        private float _telegraphTimer;

        public LanternFireSystem(
            ProjectileFactory projectileFactory,
            float fireCooldown,
            float telegraphDuration,
            LanternProjectileData projectileData,
            Vector2 shootOrigin,
            Vector2 shootDirection)
        {
            _projectileFactory = projectileFactory;
            _fireCooldown = fireCooldown;
            _telegraphDuration = telegraphDuration;
            _projectileData = projectileData;
            _shootOrigin = shootOrigin;
            _shootDirection = shootDirection;
        }

        public void OnInit(Entity entity)
        {
            _isTelegraphing = entity.IsTelegraphing;
            _isDead = entity.IsDead;
            _inDeathProcess = entity.InDeathProcess;

            _state = FireState.Cooldown;
            _cooldownTimer = _fireCooldown;
            _telegraphTimer = 0f;
        }

        public void OnFixedUpdate(float deltaTime)
        {
            // Мёртвый/умирающий фонарь не стреляет. Телеграф гасим, чтобы вьюха
            // не осталась в сжатом состоянии на трупе.
            if (_isDead.Value || _inDeathProcess.Value)
            {
                if (_isTelegraphing.Value)
                {
                    _isTelegraphing.Value = false;
                }

                return;
            }

            switch (_state)
            {
                case FireState.Cooldown:
                    TickCooldown(deltaTime);
                    break;

                case FireState.Telegraph:
                    TickTelegraph(deltaTime);
                    break;
            }
        }

        private void TickCooldown(float deltaTime)
        {
            _cooldownTimer -= deltaTime;

            if (_cooldownTimer > 0f)
            {
                return;
            }

            _state = FireState.Telegraph;
            _telegraphTimer = _telegraphDuration;
            _isTelegraphing.Value = true;
        }

        private void TickTelegraph(float deltaTime)
        {
            _telegraphTimer -= deltaTime;

            if (_telegraphTimer > 0f)
            {
                return;
            }

            _isTelegraphing.Value = false;

            Fire();

            _state = FireState.Cooldown;
            _cooldownTimer = _fireCooldown;
        }

        private void Fire()
        {
            _projectileFactory.CreateLanternProjectile(_shootOrigin, _shootDirection, _projectileData);
        }
    }
}
