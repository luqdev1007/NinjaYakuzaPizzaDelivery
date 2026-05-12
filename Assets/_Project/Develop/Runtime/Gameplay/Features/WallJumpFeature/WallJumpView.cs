using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class WallJumpView : EntityView
    {
        [Header("Components")]
        [SerializeField] private Transform _viewContainer;

        [Header("Settings")]
        [SerializeField] private float _rotationDuration = 0.4f;

        [Header("Audio")]
        [SerializeField] private SfxEvent _wallJumpSfxConfig;

        private AudioService _audioService;
        private IDisposable _disposable;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;
            // _disposable = entity.IsWallJumping.Subscribe(OnWallJumpTriggered);
        }

        private void OnWallJumpTriggered(bool old, bool current)
        {
            if (current)
            {
                PlayJumpSequence();
            }
        }

        private void PlayJumpSequence()
        {
            // Звук прыжка от стены через конфиг
            _audioService.HandleSFXEvent(_wallJumpSfxConfig);

            if (_viewContainer == null) return;

            // Сбрасываем предыдущие анимации
            _viewContainer.DOKill();

            // 1. Сальто 360 градусов (направление зависит от поворота персонажа)
            float direction = -Mathf.Sign(transform.localScale.x);
            _viewContainer.DOLocalRotate(new Vector3(0, 0, 360 * direction), _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _viewContainer.localRotation = Quaternion.identity);

            // 2. Squash эффект
            _viewContainer.DOScale(new Vector3(0.8f, 0.8f, 1f), _rotationDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _disposable?.Dispose();
            if (_viewContainer != null) _viewContainer.DOKill();
        }
    }
}