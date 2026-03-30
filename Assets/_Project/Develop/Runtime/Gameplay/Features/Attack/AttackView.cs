using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Gameplay.Features.Hero;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class AttackView : EntityView
    {
        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _attackAnimationClip;
        [SerializeField] private string _isAttackingParam = "IsAttacking";
        [SerializeField] private string _speedMultiplierParam = "AttackAnimationSpeedMultiplier";

        [Header("VFX")]
        [SerializeField] private ParticleSystem _slashParticle;

        [Header("Audio")]
        [SerializeField] private AudioCategoryType _swingCategory = AudioCategoryType.HeroAttackSwing;
        [SerializeField] private AudioCategoryType _hitCategory = AudioCategoryType.HeroAttackHit;

        private AudioService _audioService;
        private ParticleSystemRenderer _particleRenderer;
        private Transform _rootTransform;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable;

        private readonly int IsAttackingKey = Animator.StringToHash("IsAttacking");
        private readonly int SpeedMultiplierKey = Animator.StringToHash("AttackAnimationSpeedMultiplier");

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _rootTransform = entity.Transform;

            if (_slashParticle != null)
                _particleRenderer = _slashParticle.GetComponent<ParticleSystemRenderer>();

            // Настройка скорости анимации под логику
            if (_attackAnimationClip != null && entity.HasComponent<AttackProcessInitialTime>())
            {
                float speedMultiplier = _attackAnimationClip.length / entity.AttackProcessInitialTime.Value;
                _animator.SetFloat(SpeedMultiplierKey, speedMultiplier);
            }

            // Подписки
            _inAttackProcessDisposable = entity.InAttackProcess.Subscribe(OnAttackProcessChanged);
            _attackHitDisposable = entity.AttackDelayEndEvent.Subscribe(OnAttackMoment);

            if (entity.HasComponent<SuccessfulHitEvent>())
                _successfulHitDisposable = entity.GetComponent<SuccessfulHitEvent>().Value.Subscribe(OnSuccessfulHit);

            UpdateInAttackProcess(entity.InAttackProcess.Value);
        }

        private void OnAttackProcessChanged(bool old, bool current)
        {
            UpdateInAttackProcess(current);
            if (current)
            {
                // Звук взмаха всегда в начале
                _audioService.PlayRandomSfx(_swingCategory);
            }
        }

        private void OnAttackMoment()
        {
            PlaySlashEffect();
        }

        private void OnSuccessfulHit()
        {
            // Сочный звук попадания при контакте с врагом
            _audioService.PlayRandomSfx(_hitCategory);
        }

        private void PlaySlashEffect()
        {
            if (_slashParticle == null || _particleRenderer == null || _rootTransform == null) return;

            float rootScaleX = _rootTransform.localScale.x;
            Vector3 currentFlip = _particleRenderer.flip;
            currentFlip.x = rootScaleX > 0 ? 1 : 0;
            _particleRenderer.flip = currentFlip;

            _slashParticle.Stop();
            _slashParticle.Play();
        }

        private void UpdateInAttackProcess(bool value) => _animator.SetBool(IsAttackingKey, value);

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _inAttackProcessDisposable?.Dispose();
            _attackHitDisposable?.Dispose();
            _successfulHitDisposable?.Dispose();
        }
    }
}