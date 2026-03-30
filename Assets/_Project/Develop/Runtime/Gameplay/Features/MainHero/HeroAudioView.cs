using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hero
{
    public class HeroAudioView : EntityView
    {
        private AudioService _audioService;
        private string _entityId;

        private IDisposable _dashDisposable;
        private IDisposable _damageDisposable;
        private IDisposable _groundedDisposable;

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Достаем сервис из сущности (Вариант 1)
            // Если выбрал Вариант 2 — сервис уже будет в поле через Construct
            // _audioService = entity.GetComponent<AudioComponent>().Service;

            // Предположим, у тебя есть компонент с ID или типом сущности
            _entityId = "Hero";

            // 1. Рывок
            _dashDisposable = entity.IsDashing.Subscribe(OnDashChanged);

            // 2. Получение урона (подписка на ReactiveEvent в сущности)
            if (entity.HasComponent<TakeDamageRequest>())
                _damageDisposable = entity.TakeDamageEvent.Subscribe(OnTakeDamage);

            // 3. Приземление (для звука после падения/прыжка)
            _groundedDisposable = entity.IsGrounded.Subscribe(OnGroundedChanged);
        }

        private void OnDashChanged(bool old, bool isDashing)
        {
            if (isDashing)
            {
                // Логика питча остается тут, так как это чисто "сочный" визуал
                _audioService.PlaySfxVariation("AbilityImpactCharge", 1, 5, 1.3f);
            }
        }

        private void OnTakeDamage(DamageData data)
        {
            _audioService.PlaySfxByPrefix(_entityId + "TakeDamage", true);
        }

        private void OnGroundedChanged(bool old, bool isGrounded)
        {
            if (isGrounded)
                _audioService.PlayRandomSfx(AudioCategoryType.Footsteps, true, 0.8f);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _dashDisposable?.Dispose();
            _damageDisposable?.Dispose();
            _groundedDisposable?.Dispose();
        }
    }
}