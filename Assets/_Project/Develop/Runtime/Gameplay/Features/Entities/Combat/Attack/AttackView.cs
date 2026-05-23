using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    [RequireComponent(typeof(Animator))]
    public class AttackView : EntityView, IRequireAudioService
    {
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int SpeedMultiplierKey = Animator.StringToHash("AttackAnimationSpeedMultiplier");

        [Header("Animation")]
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _attackAnimationClip;

        [Header("VFX")]
        [SerializeField] private ParticleSystem[] _slashParticles;

        [Header("SFX Keys")]
        [SerializeField] private string _attackSfxKey = "AttackExecute";
        [SerializeField] private string _hitSfxKey = "AttackHitImpact"; // <-- Ключ для сочного звука попадания!

        private Transform _rootTransform;
        private int _currentSlashIndex;
        private IAudioService _audioService;

        private IDisposable _inAttackProcessDisposable;
        private IDisposable _attackHitDisposable;
        private IDisposable _successfulHitDisposable; // <-- Раскомментировали!

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

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

            // Подписываемся на ивент успешного попадания, если он сгенерирован в API
            if (entity.HasComponent<SuccessfulHitEvent>())
            {
                _successfulHitDisposable = entity.SuccessfulHitEvent.Subscribe(OnSuccessfulHit);
            }
        }

        private void OnAttackProcessChanged(bool old, bool current)
        {
            if (current)
            {
                _animator.SetTrigger(AttackTrigger);
            }
        }

        private void OnAttackMoment()
        {
            PlaySlashEffect();
            _audioService?.PlaySfx(_attackSfxKey, transform.position); // Вжух катаны!
        }

        private void OnSuccessfulHit()
        {
            // Срабатывает ТОЛЬКО когда лезвие встретило лицо призрака
            _audioService?.PlaySfx(_hitSfxKey, transform.position);
        }

        private void PlaySlashEffect()
        {
            if (_slashParticles == null || _slashParticles.Length == 0)
                return;

            ParticleSystem activeSlash = _slashParticles[_currentSlashIndex];

            if (activeSlash != null)
            {
                float yRotation = _rootTransform.localRotation.eulerAngles.y;
                float direction = Mathf.Abs(yRotation - 180f) < 1f ? -1f : 1f;

                Vector3 effectScale = activeSlash.transform.localScale;
                effectScale.x = Mathf.Abs(effectScale.x) * direction;
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
            _successfulHitDisposable?.Dispose(); // <-- Не забываем чистить за собой
        }
    }
}