using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class AttackView : EntityView
    {
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int SpeedMultiplierKey = Animator.StringToHash("AttackAnimationSpeedMultiplier");

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _attackAnimationClip;

        [Header("VFX")]
        [SerializeField] private ParticleSystem[] _slashParticles;

        private Transform _rootTransform;
        private int _currentSlashIndex;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable;

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _rootTransform = entity.Transform;

            if (_attackAnimationClip != null && entity.HasComponent<AttackProcessInitialTime>())
            {
                float speedMultiplier = _attackAnimationClip.length / entity.AttackProcessInitialTime.Value;
                _animator.SetFloat(SpeedMultiplierKey, speedMultiplier);
            }

            _inAttackProcessDisposable = entity.InAttackProcess.Subscribe(OnAttackProcessChanged);
            _attackHitDisposable = entity.AttackDelayEndEvent.Subscribe(OnAttackMoment);
        }

        private void OnAttackProcessChanged(bool old, bool current)
        {
            if (current)
            {
                _animator.SetTrigger(AttackTrigger);
            }
        }

        private void OnAttackMoment() => PlaySlashEffect();


        private void PlaySlashEffect()
        {
            ParticleSystem activeSlash = _slashParticles[_currentSlashIndex];

            if (activeSlash != null)
            {
                Vector3 effectScale = activeSlash.transform.localScale;
                effectScale.x = _rootTransform.localScale.x > 0 ? 1f : -1f;
                activeSlash.transform.localScale = effectScale;

                activeSlash.Stop();
                activeSlash.Play();
            }

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