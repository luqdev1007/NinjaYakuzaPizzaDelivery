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
        [Tooltip("Ищет в конфиге SwordSwing1, SwordSwing2...")]
        [SerializeField] private string _swingPrefix = "SwordSwing";

        [Tooltip("Ищет в конфиге EnemyHit1, EnemyHit2...")]
        [SerializeField] private string _hitPrefix = "EnemyHit";

        private AudioService _audioService;
        private ParticleSystemRenderer _particleRenderer;
        private Transform _rootTransform;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable;

        private static readonly int IsAttackingKey = Animator.StringToHash("IsAttacking");
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
                // ИСПОЛЬЗУЕМ АВТОМАТИКУ: не нужно знать количество вариаций
                _audioService.PlaySfxByPrefixAuto(_swingPrefix, UnityEngine.Random.Range(0.95f, 1.05f));
            }
        }

        private void OnAttackMoment() => PlaySlashEffect();

        private void OnSuccessfulHit()
        {
            // ИСПОЛЬЗУЕМ АВТОМАТИКУ: просто передаем префикс "EnemyHit"
            _audioService.PlaySfxByPrefixAuto(_hitPrefix, UnityEngine.Random.Range(0.95f, 1.1f));
        }

        private void PlaySlashEffect()
        {
            if (_slashParticle == null || _particleRenderer == null || _rootTransform == null) return;

            // Разворот партиклов в сторону взгляда персонажа
            float rootScaleX = _rootTransform.localScale.x;
            Vector3 currentFlip = _particleRenderer.flip;
            currentFlip.x = rootScaleX > 0 ? 0 : 1; // Поменял логику флипа под стандарт Unity
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