using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Автономный снаряд фонаря: летит прямо, гаснет об геометрию, по времени жизни
    /// или от разруба катаной/дэшем.
    ///
    /// ДЕСПАВН ОБ ГЕОМЕТРИЮ — КАСТОМ, А НЕ КОНТАКТОМ. Каждый fixed-тик бьём
    /// Linecast от ПРЕДЫДУЩЕЙ позиции к новой по BlockMask (Ground+Wall). Линия, а
    /// не точка, — обязательна: на ProjectileSpeed снаряд за тик проходит заметно
    /// больше своего радиуса и точечная проверка протуннелировала бы сквозь тонкую
    /// стену. LifeTime остаётся вторым ограничителем — он рубит снаряд, улетевший в
    /// открытое небо, где кастовать не обо что.
    ///
    /// Сюрикеновый паттерн (DeathMask + IsTouchDeathMask +
    /// DeathMaskTouchDetectorSystem) здесь СОЗНАТЕЛЬНО не используется: он ловит
    /// стену overlap'ом тела на Update, то есть туннелирует и живёт на чужом канале.
    /// Геометрию в ContactsDetectingMask не кладём вовсе — контактный буфер остаётся
    /// чисто про урон герою.
    ///
    /// IGNORE-SET НА СПАВНЕ — НЕ ОПЦИЯ, А УСЛОВИЕ РАБОТОСПОСОБНОСТИ. При
    /// Physics2D.queriesStartInColliders = 1 (в проекте именно так) каст, стартующий
    /// ВНУТРИ коллайдера, возвращает его хитом с дистанцией 0 — снаряд умирал бы в
    /// точке рождения, каждый выстрел, молча. Поэтому в OnInit (а он зовётся
    /// синхронно из EntitiesLifeContext.Add, то есть буквально в момент спавна)
    /// снимаем OverlapPoint в точке дула по ТОЙ ЖЕ маске и запоминаем всё, что
    /// попалось; каст движения эти коллайдеры пропускает.
    ///
    /// Набор фиксируется ОДИН РАЗ и не обновляется — это не оптимизация, а
    /// требование: снаряд, рождённый внутри стены, обязан из неё вылететь, а не
    /// упереться в неё изнутри. Приём — по образцу sight-check слайма
    /// (TongueSystem.TryFindBlockingHit), но там отсев считается на каждый каст
    /// заново, потому что слайм стреляет из статичной точки, а здесь старт каста
    /// уезжает вместе со снарядом.
    ///
    /// Глобальный queriesStartInColliders НЕ трогаем: его мутация посреди кадра
    /// задела бы любые другие касты в том же тике (та же мотивация, что в
    /// TongueSystem).
    ///
    /// ПОЧЕМУ FIXED, А НЕ TransformMovementSystem. Снаряд языка слайма — «тупая»
    /// сущность, её каждый тик двигает система на слайме. Здесь снаряд автономен,
    /// и по решению для фонаря его движение, каст и время жизни ведутся на
    /// fixed-канале (в отличие от Update-образца ChargedSlash/Throwable). Контактная
    /// детекция (BodyContactDetecting/Filter/DealDamage) для урона герою при этом
    /// остаётся на Update — это общие системы проекта, их канал не трогаем.
    ///
    /// ЕДИНСТВЕННЫЙ ВЛАДЕЛЕЦ РЕЛИЗА. Все три пути завершения (стена, время жизни,
    /// разруб) сходятся в один идемпотентный Release — по образцу
    /// TongueSystem.CancelAll с флагом _isReleased. EntitiesLifeContext.Release
    /// кладёт заявку в очередь, сливаемую в конце Update, поэтому сущность проживёт
    /// ещё несколько fixed-тиков живой; флаг гасит поведение немедленно, чтобы за
    /// это окно не уйти в релиз повторно.
    ///
    /// РАЗРУБ. Подписка на собственный TakeDamageRequest: у снаряда нет
    /// ApplyDamageSystem (и добавлять её нельзя — это выключит урон, см. мину в
    /// ProjectileFactory.CreateSlimeTongue), поэтому запрос на урон здесь и есть
    /// сигнал «меня разрубили».
    /// </summary>
    public class LanternProjectileSystem : IInitializableSystem, IFixedUpdatableSystem, IDisposableSystem
    {
        // Оба буфера неаллоцирующих запросов. Переполнение буфера каста безобидно
        // (лишний хит просто не рассмотрен — снаряд пролетит дальше), а вот
        // переполнение overlap'а на спавне ВОЗВРАЩАЕТ СТАРУЮ ПОЛОМКУ: не попавший в
        // набор охватывающий коллайдер убьёт снаряд в точке рождения. Поэтому на
        // втором стоит явный warning при заполнении под завязку.
        private const int CastHitsCapacity = 8;
        private const int SpawnOverlapCapacity = 8;

        // Короче этого шага каст вырождается (Linecast нулевой длины). Тик, в
        // котором снаряд не сдвинулся, геометрию не проверяет.
        private const float MinCastDistance = 0.0001f;

        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly LayerMask _blockMask;

        private readonly RaycastHit2D[] _castHits = new RaycastHit2D[CastHitsCapacity];
        private readonly Collider2D[] _spawnOverlaps = new Collider2D[SpawnOverlapCapacity];

        // Reference-равенство Collider2D — то, что нужно: сверяем конкретные
        // экземпляры, попавшиеся в точке дула.
        private readonly HashSet<Collider2D> _ignoredColliders = new HashSet<Collider2D>();

        private Entity _entity;
        private Transform _transform;

        private ContactFilter2D _blockFilter;

        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<float> _lifeTime;

        private ReactiveEvent<DamageData> _takeDamageRequest;
        private IDisposable _takeDamageDisposable;

        private bool _isReleased;

        public LanternProjectileSystem(EntitiesLifeContext entitiesLifeContext, LayerMask blockMask)
        {
            _entitiesLifeContext = entitiesLifeContext;
            _blockMask = blockMask;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _transform = entity.Transform;

            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _lifeTime = entity.LifeTime;

            _takeDamageRequest = entity.TakeDamageRequest;
            _takeDamageDisposable = _takeDamageRequest.Subscribe(OnTakeDamage);

            // useTriggers = true — как в sight-check слайма: глобальный
            // Physics2D.queriesHitTriggers в проекте включён, и триггерная геометрия
            // на Ground/Wall обязана считаться преградой наравне с твёрдой.
            _blockFilter = new ContactFilter2D();
            _blockFilter.useTriggers = true;
            _blockFilter.SetLayerMask(_blockMask);

            _isReleased = false;

            CaptureIgnoredColliders();
        }

        public void OnFixedUpdate(float deltaTime)
        {
            if (_isReleased)
            {
                return;
            }

            if (_transform == null)
            {
                Release();
                return;
            }

            Vector2 currentPosition = _transform.position;
            Vector2 step = _moveDirection.Value * (_moveSpeed.Value * deltaTime);
            Vector2 nextPosition = currentPosition + step;

            if (TryFindBlockingHit(currentPosition, nextPosition, out RaycastHit2D _))
            {
                // Позицию НЕ двигаем: снаряд гаснет там, где успел долететь, а не
                // втыкается видимой частью внутрь стены.
                Release();
                return;
            }

            // Translate, а не присваивание position: Vector2 → Vector3 обнулил бы z.
            _transform.Translate(step, Space.World);

            if (_lifeTime.Value > 0f)
            {
                _lifeTime.Value -= deltaTime;
            }

            if (_lifeTime.Value <= 0f)
            {
                Release();
            }
        }

        /// <summary>
        /// Снимок охватывающих коллайдеров в точке вылета. Зовётся ровно один раз,
        /// из OnInit. Пустой набор — штатная ситуация: дуло в воздухе.
        /// </summary>
        private void CaptureIgnoredColliders()
        {
            _ignoredColliders.Clear();

            if (_transform == null)
            {
                return;
            }

            Vector2 spawnPoint = _transform.position;

            int count = Physics2D.OverlapPoint(spawnPoint, _blockFilter, _spawnOverlaps);

            for (int i = 0; i < count; i++)
            {
                Collider2D collider = _spawnOverlaps[i];

                if (collider == null)
                {
                    continue;
                }

                _ignoredColliders.Add(collider);
            }

            if (count >= SpawnOverlapCapacity)
            {
                Debug.LogWarning(
                    $"[LanternProjectile] Буфер overlap'а на спавне заполнен целиком " +
                    $"({count}/{SpawnOverlapCapacity}) в точке {spawnPoint}. Часть охватывающих " +
                    $"коллайдеров могла не попасть в ignore-set — снаряд рискует гаснуть " +
                    $"в точке вылета. Подними SpawnOverlapCapacity или сузь BlockMask.");
            }
        }

        /// <summary>
        /// Неаллоцирующий каст с отсевом коллайдеров из ignore-set.
        ///
        /// Возвращается БЛИЖАЙШИЙ валидный хит: порядок результатов в буфере не
        /// гарантирован, поэтому минимум по дистанции ищем явно. Для самого релиза
        /// хватило бы и bool, но ближайший хит — это точка, где снаряд реально
        /// упёрся; она нужна любому будущему VFX искры об стену.
        /// </summary>
        private bool TryFindBlockingHit(Vector2 start, Vector2 end, out RaycastHit2D blockingHit)
        {
            blockingHit = default;

            Vector2 delta = end - start;

            if (delta.sqrMagnitude < MinCastDistance * MinCastDistance)
            {
                return false;
            }

            int count = Physics2D.Linecast(start, end, _blockFilter, _castHits);
            float closestDistance = float.MaxValue;
            bool found = false;

            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = _castHits[i];

                if (hit.collider == null)
                {
                    continue;
                }

                if (_ignoredColliders.Contains(hit.collider))
                {
                    continue;
                }

                if (hit.distance >= closestDistance)
                {
                    continue;
                }

                closestDistance = hit.distance;
                blockingHit = hit;
                found = true;
            }

            return found;
        }

        private void OnTakeDamage(DamageData damageData)
        {
            Release();
        }

        /// <summary>
        /// Идемпотентный релиз: повторный вызов не подаст вторую заявку (двойной
        /// Release дал бы двойной Dispose сущности и повторный прогон её OnDispose).
        /// </summary>
        private void Release()
        {
            if (_isReleased)
            {
                return;
            }

            _isReleased = true;

            _takeDamageDisposable?.Dispose();
            _takeDamageDisposable = null;

            _entitiesLifeContext.Release(_entity);
        }

        public void OnDispose()
        {
            _takeDamageDisposable?.Dispose();
            _takeDamageDisposable = null;

            _ignoredColliders.Clear();
        }
    }
}
