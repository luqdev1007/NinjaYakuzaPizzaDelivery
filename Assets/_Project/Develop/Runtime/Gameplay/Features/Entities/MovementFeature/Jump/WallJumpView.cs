using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment; // Добавлено
using DG.Tweening;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Jump
{
    public class WallJumpView : EntityView, IRequireAudioService // Подключаем интерфейс
    {
        [Header("DOTween Settings")]
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _rotationDuration = 0.4f;

        [Header("SFX Keys")]
        [SerializeField] private string _wallJumpSfxKey = "WallJump"; // Ключ для звука

        private IDisposable _disposable;
        private IAudioService _audioService; // Ссылка на сервис

        // Внедрение зависимости
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _disposable = entity.IsWallJumping.Subscribe(OnWallJumpTriggered);
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
            if (_viewContainer == null) return;

            // 1. Убиваем текущие твины и ЖЕСТКО СБРАСЫВАЕМ трансформ в дефолт,
            // чтобы избежать накопления ошибок масштаба при быстром спаме прыжков
            _viewContainer.DOKill();
            _viewContainer.localRotation = Quaternion.identity;
            _viewContainer.localScale = Vector3.one;

            // 2. Считаем направление вращения на основе скейла
            float direction = -Mathf.Sign(transform.localScale.x);

            // 3. Крутим на 360 градусов
            _viewContainer.DOLocalRotate(new Vector3(0, 0, 360 * direction), _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _viewContainer.localRotation = Quaternion.identity);

            // 4. Сжимаем-разжимаем (Squash & Stretch) через Yoyo
            _viewContainer.DOScale(new Vector3(0.8f, 0.8f, 1f), _rotationDuration * 0.5f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutQuad);

            // 5. ВОСПРОИЗВОДИМ ЗВУК ОТТАЛКИВАНИЯ
            _audioService?.PlaySfx(_wallJumpSfxKey, transform.position);
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);

            _disposable?.Dispose();

            if (_viewContainer != null)
            {
                _viewContainer.DOKill();
                _viewContainer.localRotation = Quaternion.identity;
                _viewContainer.localScale = Vector3.one;
            }
        }
    }
}