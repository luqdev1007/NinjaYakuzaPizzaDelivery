using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Dialog
{
    public class SkipLabelView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image _glowImage;

        private Tween _shakeTween;
        private Tween _holdTween;
        private float _idleTimer;

        private const float IdleThreshold = 5f;

        public void Initialize()
        {
            StopAnims();
            if (_group != null) _group.alpha = 0;
            if (_root != null) _root.localScale = Vector3.zero;
            _idleTimer = 0;
        }

        public void Show()
        {
            if (_group == null || _root == null) return;
            StopAnims();
            _group.DOFade(1f, 0.4f);
            _root.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            _idleTimer = 0;
        }

        public void Hide()
        {
            StopAnims();
            if (_group != null) _group.DOFade(0f, 0.3f);
            if (_root != null) _root.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        }

        public void UpdateIdle(float deltaTime)
        {
            if (_root == null) return;
            _idleTimer += deltaTime;

            if (_idleTimer >= IdleThreshold && (_shakeTween == null || !_shakeTween.IsPlaying()))
            {
                if (_shakeTween != null) _shakeTween.Kill();
                _shakeTween = _root.DOShakeAnchorPos(2f, 5, 10).SetLoops(-1);
            }
        }

        public void ResetIdle()
        {
            _idleTimer = 0;
            if (_shakeTween != null && _shakeTween.IsPlaying())
            {
                _shakeTween.Kill();
                _root.DOAnchorPos(Vector2.zero, 0.2f);
            }
        }

        public void StartHoldProgress(float duration)
        {
            if (_root == null) return;
            ResetIdle();
            if (_holdTween != null) _holdTween.Kill();
            _holdTween = _root.DOScale(1.5f, duration).SetEase(Ease.InQuad);
        }

        public void StopHoldProgress()
        {
            if (_holdTween != null) _holdTween.Kill();
            if (_root != null) _root.DOScale(1f, 0.2f);
        }

        private void StopAnims()
        {
            if (_shakeTween != null) _shakeTween.Kill();
            if (_holdTween != null) _holdTween.Kill();
            if (_root != null) _root.DOKill();
            if (_group != null) _group.DOKill();
            if (_glowImage != null) _glowImage.DOKill();
        }
    }
}