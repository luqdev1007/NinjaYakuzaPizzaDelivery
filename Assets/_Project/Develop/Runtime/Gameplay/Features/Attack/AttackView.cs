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
        [Tooltip("Помести сюда 3 разных объекта с партиклами слэша")]
        [SerializeField] private ParticleSystem[] _slashParticles;

        [Header("Audio")]
        [SerializeField] private SfxEvent _swingSoundConfig;
        [SerializeField] private SfxEvent _hitSondConfig;


        private AudioService _audioService;
        private Transform _rootTransform;
        private int _currentSlashIndex;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable;

        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int SpeedMultiplierKey = Animator.StringToHash("AttackAnimationSpeedMultiplier");

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            /*
            _audioService = entity.GetComponent<AudioComponent>().Service;
            _rootTransform = entity.Transform;

            if (_attackAnimationClip != null && entity.HasComponent<AttackProcessInitialTime>())
            {
                float speedMultiplier = _attackAnimationClip.length / entity.AttackProcessInitialTime.Value;
                _animator.SetFloat(SpeedMultiplierKey, speedMultiplier);
            }

            _inAttackProcessDisposable = entity.InAttackProcess.Subscribe(OnAttackProcessChanged);
            _attackHitDisposable = entity.AttackDelayEndEvent.Subscribe(OnAttackMoment);

            if (entity.HasComponent<SuccessfulHitEvent>())
                _successfulHitDisposable = entity.GetComponent<SuccessfulHitEvent>().Value.Subscribe(OnSuccessfulHit);
            */
        }

        private void OnAttackProcessChanged(bool old, bool current)
        {
            if (current)
            {
                _animator.SetTrigger(AttackTrigger);
                _audioService.HandleSFXEvent(_swingSoundConfig);
            }
        }

        private void OnAttackMoment() => PlaySlashEffect();

        private void OnSuccessfulHit()
        {
            _audioService.HandleSFXEvent(_hitSondConfig);
        }

        private void PlaySlashEffect()
        {
            if (_slashParticles == null || _slashParticles.Length == 0 || _rootTransform == null) return;

            // Выбираем текущий эффект
            ParticleSystem activeSlash = _slashParticles[_currentSlashIndex];

            if (activeSlash != null)
            {
                // Настраиваем поворот (скейл)
                Vector3 effectScale = activeSlash.transform.localScale;
                effectScale.x = _rootTransform.localScale.x > 0 ? 1f : -1f;
                activeSlash.transform.localScale = effectScale;

                activeSlash.Stop();
                activeSlash.Play();
            }

            // Переходим к следующему индексу (зацикливаем через остаток от деления)
            _currentSlashIndex = (_currentSlashIndex + 1) % _slashParticles.Length;
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