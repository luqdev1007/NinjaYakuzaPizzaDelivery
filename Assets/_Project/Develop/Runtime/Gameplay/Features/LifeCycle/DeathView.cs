using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; 

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle
{
    [RequireComponent(typeof(Animator))]
    public class DeathView : EntityView, IRequireAudioService 
    {
        private static readonly int IsDyingKey = Animator.StringToHash("IsDying");

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("Audio")]
        [SerializeField] private string _deathSfxKey = "Death"; 

        private IAudioService _audioService; 
        private IDisposable _isDeadChangedDisposable;

        private void OnValidate()
        {
            _animator ??= GetComponent<Animator>();
        }

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isDeadChangedDisposable = entity.IsDead.Subscribe(OnIsDeadChanged);
        }

        private void OnIsDeadChanged(bool old, bool isDead)
        {
            _animator.SetBool(IsDyingKey, isDead);

            if (isDead && !old)
            {
                _audioService?.PlaySfx(_deathSfxKey, transform.position);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _isDeadChangedDisposable?.Dispose();
        }
    }
}