using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hero
{
    public class HeroAudioView : EntityView
    {
        private AudioService _audioService;
        private string _entityPrefix;

        private IDisposable _dashDisposable;
        private IDisposable _damageDisposable;
        private IDisposable _groundedDisposable;

        protected override void OnEntityStartedWork(Entity entity)
        {
            // Раскомментировал получение сервиса, без этого будет NullReferenceException
            _audioService = entity.GetComponent<AudioComponent>().Service;

            // Используем префикс, который соответствует именам в конфиге (например, "HeroTakeDamage1")
            _entityPrefix = "Hero";

            // 1. Рывок
            _dashDisposable = entity.IsDashing.Subscribe(OnDashChanged);

            // 2. Получение урона
            if (entity.HasComponent<TakeDamageRequest>())
                _damageDisposable = entity.TakeDamageEvent.Subscribe(OnTakeDamage);

            // 3. Приземление
            _groundedDisposable = entity.IsGrounded.Subscribe(OnGroundedChanged);
        }

        private void OnDashChanged(bool old, bool isDashing)
        {
            if (isDashing)
            {
                // Используем PlaySfxVariation, так как тут ты жестко задал диапазон 1-5
                _audioService.PlaySfxVariation("AbilityImpactCharge", 1, 5, 1.3f);
            }
        }

        private void OnTakeDamage(DamageData data)
        {
            // Исправлено: заменено на PlaySfxByPrefixAuto
            // Метод сам найдет HeroTakeDamage1, HeroTakeDamage2 и т.д.
            _audioService.PlaySfxByPrefixAuto(_entityPrefix + "TakeDamage", UnityEngine.Random.Range(0.9f, 1.1f));
        }

        private void OnGroundedChanged(bool old, bool isGrounded)
        {
            // Приземление: играем звук шага, но чуть тише или с другим питчем
            if (isGrounded)
                _audioService.PlayRandomSfx(AudioCategoryType.Movement, true, 0.8f);
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

