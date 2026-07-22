using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies.Lantern
{
    /// <summary>
    /// Автономный снаряд фонаря: летит прямо, гаснет по времени жизни, об стену
    /// или от разруба катаной/дэшем.
    ///
    /// ПОЧЕМУ FIXED, А НЕ TransformMovementSystem. Снаряд языка слайма — «тупая»
    /// сущность, её каждый тик двигает система на слайме. Здесь снаряд автономен,
    /// и по решению для фонаря его движение и время жизни ведутся на fixed-канале
    /// (в отличие от Update-образца ChargedSlash/Throwable). Контактная детекция
    /// (BodyContactDetecting/Filter/DealDamage/DeathMaskTouch) при этом остаётся на
    /// Update — это общие системы проекта, их канал не трогаем. Рассинхрон каналов
    /// штатен: IsTouchDeathMask ставится на Update, читается здесь на ближайшем
    /// fixed-тике, задержка максимум один тик.
    ///
    /// ЕДИНСТВЕННЫЙ ВЛАДЕЛЕЦ РЕЛИЗА. Все три пути завершения (время жизни, стена,
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
        private readonly EntitiesLifeContext _entitiesLifeContext;

        private Entity _entity;
        private Transform _transform;

        private ReactiveVariable<Vector2> _moveDirection;
        private ReactiveVariable<float> _moveSpeed;
        private ReactiveVariable<float> _lifeTime;
        private ReactiveVariable<bool> _isTouchDeathMask;

        private ReactiveEvent<DamageData> _takeDamageRequest;
        private IDisposable _takeDamageDisposable;

        private bool _isReleased;

        public LanternProjectileSystem(EntitiesLifeContext entitiesLifeContext)
        {
            _entitiesLifeContext = entitiesLifeContext;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _transform = entity.Transform;

            _moveDirection = entity.MoveDirection;
            _moveSpeed = entity.MoveSpeed;
            _lifeTime = entity.LifeTime;
            _isTouchDeathMask = entity.IsTouchDeathMask;

            _takeDamageRequest = entity.TakeDamageRequest;
            _takeDamageDisposable = _takeDamageRequest.Subscribe(OnTakeDamage);

            _isReleased = false;
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

            Vector2 direction = _moveDirection.Value;

            _transform.Translate(direction * _moveSpeed.Value * deltaTime, Space.World);

            if (_lifeTime.Value > 0f)
            {
                _lifeTime.Value -= deltaTime;
            }

            if (_lifeTime.Value <= 0f)
            {
                Release();
                return;
            }

            // Стену детектит DeathMaskTouchDetectorSystem на Update — здесь только
            // читаем результат.
            if (_isTouchDeathMask.Value)
            {
                Release();
            }
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
        }
    }
}
