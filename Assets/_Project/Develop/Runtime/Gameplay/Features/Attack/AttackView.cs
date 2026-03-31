using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class AttackView : EntityView
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _attackAnimationClip;

        [Header("VFX")]
        [SerializeField] private ParticleSystem _slashParticle;

        [Header("Audio (Auto-detected)")]
        [SerializeField] private string _swingPrefix = "SwordSwing";
        [SerializeField] private string _hitPrefix = "EnemyHit";

        private AudioService _audioService;
        private ParticleSystemRenderer _particleRenderer;
        private Transform _rootTransform;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable;

        // Имя параметра в аниматоре должно быть типа Trigger
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int SpeedMultiplierKey = Animator.StringToHash("AttackAnimationSpeedMultiplier");

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _rootTransform = entity.Transform;

            if (_slashParticle != null)
                _particleRenderer = _slashParticle.GetComponent<ParticleSystemRenderer>();

            // Настройка скорости анимации
            if (_attackAnimationClip != null && entity.HasComponent<AttackProcessInitialTime>())
            {
                float speedMultiplier = _attackAnimationClip.length / entity.AttackProcessInitialTime.Value;
                _animator.SetFloat(SpeedMultiplierKey, speedMultiplier);
            }

            // Подписываемся на начало процесса атаки
            _inAttackProcessDisposable = entity.InAttackProcess.Subscribe(OnAttackProcessChanged);
            _attackHitDisposable = entity.AttackDelayEndEvent.Subscribe(OnAttackMoment);

            if (entity.HasComponent<SuccessfulHitEvent>())
                _successfulHitDisposable = entity.GetComponent<SuccessfulHitEvent>().Value.Subscribe(OnSuccessfulHit);
        }

        private void OnAttackProcessChanged(bool old, bool current)
        {
            // Если флаг стал true — это момент начала взмаха
            if (current)
            {
                // Генерируем триггер для аниматора
                _animator.SetTrigger(AttackTrigger);

                // Звук взмаха
                _audioService.PlaySfxByPrefixAuto(_swingPrefix, UnityEngine.Random.Range(0.95f, 1.05f));
            }
        }

        private void OnAttackMoment() => PlaySlashEffect();

        private void OnSuccessfulHit()
        {
            _audioService.PlaySfxByPrefixAuto(_hitPrefix, UnityEngine.Random.Range(0.95f, 1.1f));
        }

        private void PlaySlashEffect()
        {
            if (_slashParticle == null || _particleRenderer == null || _rootTransform == null) return;

            float rootScaleX = _rootTransform.localScale.x;
            Vector3 currentFlip = _particleRenderer.flip;
            currentFlip.x = rootScaleX > 0 ? 0 : 1;
            _particleRenderer.flip = currentFlip;

            _slashParticle.Stop();
            _slashParticle.Play();
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _inAttackProcessDisposable?.Dispose();
            _attackHitDisposable?.Dispose();
            _successfulHitDisposable?.Dispose();
        }
    }
}