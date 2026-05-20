using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.GrappleFeature
{
    public class GrappleView : EntityView, IRequireAudioService
    {
        private static readonly int IsThrowingHookKey = Animator.StringToHash("IsThrowingHook");

        [SerializeField] private Animator _animator;

        [Header("Audio Keys")]
        [SerializeField] private string _shootSfxKey = "HookShoot";
        [SerializeField] private string _loopSfxKey = "HookLoop";
        [SerializeField] private string _breakSfxKey = "HookBreak";

        private IAudioService _audioService;
        private IReadOnlyVariable<bool> _isGrappling;
        private IDisposable _disposable;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        private void OnValidate() => _animator ??= GetComponent<Animator>();

        protected override void OnEntityStartedWork(Entity entity)
        {
            _isGrappling = entity.GetComponent<IsGrappling>().Value;
            _disposable = _isGrappling.Subscribe(OnGrappleStateChanged);

            SyncState(_isGrappling.Value, playEffects: false);
        }

        private void OnGrappleStateChanged(bool oldValue, bool newValue)
        {
            SyncState(newValue, playEffects: true);
        }

        private void SyncState(bool isGrappling, bool playEffects)
        {
            if (_animator != null)
                _animator.SetBool(IsThrowingHookKey, isGrappling);

            if (!playEffects || _audioService == null) 
                return;

            if (isGrappling)
            {
                _audioService.PlaySfx(_shootSfxKey);
                _audioService.PlaySfxLoop(_loopSfxKey);
            }
            else
            {
                _audioService.StopSfx(_loopSfxKey);
                _audioService.PlaySfx(_breakSfxKey);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _disposable?.Dispose();

            if (_audioService != null)
                _audioService.StopSfx(_loopSfxKey);
        }
    }
}