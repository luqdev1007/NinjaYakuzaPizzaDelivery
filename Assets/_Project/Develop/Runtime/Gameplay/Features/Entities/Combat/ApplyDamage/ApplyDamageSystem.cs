using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.RandomManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage
{
    public class ApplyDamageSystem : IInitializableSystem, IDisposableSystem
    {
        private const float MinChancePercent = 0f;
        private const float MaxChancePercent = 100f;

        private ICompositeCondition _canApplyDamage;

        private ReactiveEvent<DamageData> _damageRequest;
        private ReactiveEvent<DamageData> _damageEvent;

        private ReactiveVariable<float> _currentHealth;

        // Уклонение OPT-IN. Оба поля остаются null у сущностей без компонентов —
        // это штатное состояние для призрака, слайма, фонаря и пропсов, а не
        // недонастройка. null здесь читается как «броска нет вообще», а НЕ как
        // «шанс равен нулю»: разница в том, что мы даже не трогаем рандом и не
        // сдвигаем засеянную последовательность у тех, кто в фиче не участвует.
        private ReactiveVariable<float> _evasionChance;
        private ReactiveEvent _evadedEvent;

        // Уклонение отменяет урон, значит решение реплей-чувствительное и обязано
        // идти из засеянного потока — та же логика, что у DoubleAttackSystem.
        private readonly IGameplayRandom _random;

        private IDisposable _requestDisposable;

        public ApplyDamageSystem(IGameplayRandom random)
        {
            _random = random;
        }

        public void OnInit(Entity entity)
        {
            _canApplyDamage = entity.CanApplyDamage;

            _damageRequest = entity.TakeDamageRequest;
            _damageEvent = entity.TakeDamageEvent;

            _currentHealth = entity.CurrentHealth;

            // TryGet один раз в OnInit, а не на каждый хит: остальные системы
            // проекта кешируют ссылки ровно так же.
            if (entity.TryGetEvasionChance(out ReactiveVariable<float> evasionChance))
            {
                _evasionChance = evasionChance;
            }

            if (entity.TryGetEvadedEvent(out ReactiveEvent evadedEvent))
            {
                _evadedEvent = evadedEvent;
            }

            _requestDisposable = _damageRequest.Subscribe(OnDamageRequest);
        }

        private void OnDamageRequest(DamageData damage)
        {
            if (_canApplyDamage.Evaluate() == false)
                return;

            // ПОРЯДОК КРИТИЧЕН. Бросок стоит ПОСЛЕ гейта и ДО декремента.
            //
            // После гейта — потому что уклоняться от хита, который и так не
            // проходит (i-frames, дэш, пике, спавн, смерть), бессмысленно: это
            // жгло бы броски впустую и делало бы фактический шанс ниже
            // заявленного в конфиге.
            //
            // До декремента — потому что уклонение обязано отменять хит ЦЕЛИКОМ.
            // Ниже единственная точка, где сущность теряет здоровье, и
            // единственный инвок TakeDamageEvent; выйдя здесь, мы гасим разом и
            // потерю HP, и весь хвост подписчиков: DamageCooldown (i-frames),
            // штраф стиля, hit flash, тряску пиццы, звук получения урона.
            if (IsEvaded())
            {
                _evadedEvent?.Invoke();

                return;
            }

            _currentHealth.Value = MathF.Max(_currentHealth.Value - damage.Amount, 0);

            _damageEvent.Invoke(damage);
        }

        private bool IsEvaded()
        {
            if (_evasionChance == null)
                return false;

            return IsChanceProceed(_evasionChance.Value);
        }

        /// <summary>
        /// Бросок шанса в процентах (0..100).
        ///
        /// Границы обрабатываются ЯВНО, а не арифметикой. У DoubleAttackSystem
        /// формула Range(0, 100) &lt;= percent, и там включающий максимум в паре с
        /// &lt;= даёт щель: при percent == 0 бросок формально проходит, если выпадет
        /// ровно 0.0f. Для двойной атаки с базовыми 45% это шум, а у уклонения
        /// ноль — штатное значение любого не-героя и любого героя без стата, и
        /// «шанс 0» обязан означать НИКОГДА, без оговорок.
        ///
        /// Поэтому здесь: 0 и 100 закорочены на константы, а в середине сравнение
        /// СТРОГОЕ (&lt;), так что верхняя граница диапазона рандома не может
        /// подарить лишний прок.
        /// </summary>
        private bool IsChanceProceed(float chancePercent)
        {
            if (chancePercent <= MinChancePercent)
                return false;

            if (chancePercent >= MaxChancePercent)
                return true;

            return _random.Range(MinChancePercent, MaxChancePercent) < chancePercent;
        }

        public void OnDispose() => _requestDisposable.Dispose();
    }
}
