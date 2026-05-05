using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.PlungeFeature
{
    public class PlungeSquashView : EntityView
    {
        [SerializeField] private Transform _viewContainer;
        [SerializeField] private float _stretchY = 1.3f;
        [SerializeField] private float _squashY = 0.7f;
        [SerializeField] private float _duration = 0.2f;

        private Vector3 _baseScale;
        private Tweener _currentTween;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _baseScale = _viewContainer.localScale;

            entity.IsPlunging.Subscribe((_, active) => { if (active) ApplyStretch(); });
            entity.IsGrounded.Subscribe((_, grounded) => { if (grounded) ApplySquash(); });
        }

        private void ApplyStretch()
        {
            _currentTween?.Kill();

            _viewContainer.DOScaleY(_baseScale.y * _stretchY, 0.15f).SetEase(Ease.OutQuad);
            _viewContainer.DOScaleX(_baseScale.x * (2f - _stretchY), 0.15f).SetEase(Ease.OutQuad);
        }

        private void ApplySquash()
        {
            _currentTween?.Kill();

            _currentTween = _viewContainer.DOScaleY(_baseScale.y * _squashY, _duration * 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
                .OnUpdate(() => {
                    float currentY = _viewContainer.localScale.y;
                    float diff = currentY / _baseScale.y;
                    _viewContainer.localScale = new Vector3(_baseScale.x * (2f - diff), currentY, _baseScale.z);
                });
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _currentTween?.Kill();
            _viewContainer.localScale = _baseScale;
        }
    }
}