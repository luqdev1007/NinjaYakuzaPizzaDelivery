using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.Projectiles;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Ритмичная стрельба фонаря. По приватному таймеру взводит телеграф, по концу
    /// телеграфа спавнит снаряд В СТОРОНУ ГЕРОЯ и уходит на кулдаун.
    ///
    /// РАДИУСНЫЙ ГЕЙТ. Фонарь стреляет, только когда герой в радиусе (простая
    /// дистанция от тела фонаря, без отдельной системы — сервис героя тут и так
    /// есть). Вне радиуса РИТМ ЗАМОРОЖЕН: таймер не тикает, начатый телеграф
    /// гасится. На входе героя в радиус кулдаун продолжается с замороженного
    /// значения → перед выстрелом всегда играет ПОЛНЫЙ телеграф, мгновенного
    /// выстрела из накопленного таймера не бывает (Fire вызывается только по концу
    /// телеграфа). Заморозка (а не сброс) сохраняет per-instance фазу из блока E:
    /// соседние фонари, входящие в радиус одновременно, продолжают со своих разных
    /// значений и не залпят синхронно.
    ///
    /// ПРИЦЕЛ — СНАПШОТ В МОМЕНТ ВЫСТРЕЛА. Направление считается ровно один раз,
    /// когда снаряд рождается: вектор от точки дула к ТЕКУЩЕЙ позиции героя,
    /// нормализованный. БЕЗ упреждения по скорости героя и БЕЗ самонаведения —
    /// дальше снаряд летит прямо, MoveDirection после спавна не трогается.
    ///
    /// ГЕРОЙ НЕДОСТУПЕН — ВЫСТРЕЛ ПРОПУСКАЕТСЯ. Если в момент выстрела героя нет
    /// (сервис пуст, сущность или её Transform уничтожены), снаряд не спавнится,
    /// но ритм не ломается: телеграф отыгрался, таймер сбрасывается на кулдаун.
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
    /// телеграф-состоянии. Проверка в начале тика, ПЕРЕД радиусным гейтом.
    ///
    /// ТОЧКА ВЫЛЕТА приезжает в конструктор из authoring (per-instance со сцены),
    /// уставки ритма/радиуса и снаряда — из LanternConfig. Компонентов под них не
    /// заводим: иных читателей нет, а реактивка без подписчика против правил проекта
    /// (то же решение, что по приватным таймерам TongueSystem).
    /// </summary>
    public class LanternFireSystem : IInitializableSystem, IFixedUpdatableSystem
    {
        private enum FireState
        {
            Cooldown,
            Telegraph
        }

        // Ниже этого модуля вектор дуло→герой считается вырожденным (герой ровно в
        // точке вылета) и выстрел пропускается — normalized иначе вернул бы ноль.
        private const float DegenerateVectorThreshold = 0.0001f;

        private readonly ProjectileFactory _projectileFactory;
        private readonly MainHeroHolderService _mainHeroHolderService;

        private readonly float _fireCooldown;
        private readonly float _telegraphDuration;
        private readonly float _fireRadius;
        private readonly LanternProjectileData _projectileData;
        private readonly Vector2 _shootOrigin;

        private Transform _transform;

        private ReactiveVariable<bool> _isTelegraphing;
        private ReactiveVariable<bool> _isDead;
        private ReactiveVariable<bool> _inDeathProcess;

        private FireState _state;
        private float _cooldownTimer;
        private float _telegraphTimer;

        public LanternFireSystem(
            ProjectileFactory projectileFactory,
            MainHeroHolderService mainHeroHolderService,
            float fireCooldown,
            float telegraphDuration,
            float fireRadius,
            LanternProjectileData projectileData,
            Vector2 shootOrigin)
        {
            _projectileFactory = projectileFactory;
            _mainHeroHolderService = mainHeroHolderService;
            _fireCooldown = fireCooldown;
            _telegraphDuration = telegraphDuration;
            _fireRadius = fireRadius;
            _projectileData = projectileData;
            _shootOrigin = shootOrigin;
        }

        public void OnInit(Entity entity)
        {
            _transform = entity.Transform;

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
                CancelTelegraph();
                return;
            }

            // Радиусный гейт: вне радиуса ритм заморожен, таймер не тикает.
            // Начатый телеграф гасим (иначе squash застынет, пока герой далеко).
            if (IsHeroInRadius() == false)
            {
                CancelTelegraph();
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

        /// <summary>
        /// Гасит начатый телеграф и откатывает фазу в Cooldown, НЕ трогая
        /// _cooldownTimer (заморозка, а не сброс). Идемпотентна.
        /// </summary>
        private void CancelTelegraph()
        {
            if (_state == FireState.Telegraph)
            {
                _state = FireState.Cooldown;
            }

            if (_isTelegraphing.Value)
            {
                _isTelegraphing.Value = false;
            }
        }

        private bool IsHeroInRadius()
        {
            Entity hero = ResolveHero();

            if (hero == null)
            {
                return false;
            }

            if (_transform == null)
            {
                return false;
            }

            Vector2 toHero = (Vector2)hero.Transform.position - (Vector2)_transform.position;

            return toHero.sqrMagnitude <= _fireRadius * _fireRadius;
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

            // Fire сам решает, спавнить снаряд или пропустить (герой недоступен).
            // Таймер сбрасывается в любом случае — ритм фонаря не зависит от того,
            // попался ли герой на прицел.
            Fire();

            _state = FireState.Cooldown;
            _cooldownTimer = _fireCooldown;
        }

        /// <summary>
        /// Снапшот направления на героя и спавн снаряда. Пропускается, если герой
        /// недоступен или стоит ровно в точке дула (вырожденный вектор).
        /// </summary>
        private void Fire()
        {
            Entity hero = ResolveHero();

            if (hero == null)
            {
                return;
            }

            Vector2 toHero = (Vector2)hero.Transform.position - _shootOrigin;

            if (toHero.sqrMagnitude < DegenerateVectorThreshold * DegenerateVectorThreshold)
            {
                return;
            }

            Vector2 direction = toHero.normalized;

            _projectileFactory.CreateLanternProjectile(_shootOrigin, direction, _projectileData);
        }

        /// <summary>
        /// Ссылка на героя берётся В МОМЕНТ ЧТЕНИЯ, не кэшируется: враги создаются
        /// раньше героя, и сервис не обнуляет ссылку после его смерти — сама Entity
        /// остаётся живым C#-объектом, а Transform к этому моменту уже уничтожен.
        /// Обе проверки обязательны (тот же приём, что в TongueSystem.ResolveHero).
        /// </summary>
        private Entity ResolveHero()
        {
            if (_mainHeroHolderService == null)
            {
                return null;
            }

            Entity hero = _mainHeroHolderService.MainHero;

            if (hero == null)
            {
                return null;
            }

            if (hero.Transform == null)
            {
                return null;
            }

            return hero;
        }
    }
}
