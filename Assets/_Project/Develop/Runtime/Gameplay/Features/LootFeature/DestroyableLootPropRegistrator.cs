using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using UnityEngine;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class DestroyableLootPropRegistrator : MonoEntityRegistrator
    {
        [Header("Health Settings")]
        [SerializeField] private float _maxHealth = 1f;
        [SerializeField] private float _damageCooldown = 0.1f;

        [Header("Loot Settings")]
        [SerializeField] private LootTableConfig _lootTable;

        public override void Register(Entity entity)
        {
            // components
            entity
               .AddMaxHealth(new ReactiveVariable<float>(_maxHealth))
               .AddCurrentHealth(new ReactiveVariable<float>(_maxHealth))
               .AddDamageCooldown(new ReactiveVariable<float>(_damageCooldown))
               .AddDamageCooldownTimer(new ReactiveVariable<float>(_damageCooldown)) // 0?

               .AddTakeDamageRequest(new ReactiveEvent<DamageData>())
               .AddTakeDamageEvent(new ReactiveEvent<DamageData>())

               .AddLootIsDropped(new ReactiveVariable<bool>(false))
               ;

            // conditions
            ICompositeCondition canDropLoot = new CompositeCondition()
                .Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0))
                ;

            ICompositeCondition canApplyDamage = new CompositeCondition()
                .Add(new FuncCondition(() => entity.DamageCooldownTimer.Value <= 0))
                .Add(new FuncCondition(() => entity.CurrentHealth.Value > 0))
                ;

            entity
                .AddCanApplyDamage(canApplyDamage)
                .AddCanDropLoot(canDropLoot)
                ;

            // systems
            entity
                .AddSystem(new ApplyDamageSystem())
                .AddSystem(new DropLootSystem(_container.Resolve<DropLootService>(), _lootTable));
            ;
        }
    }
}