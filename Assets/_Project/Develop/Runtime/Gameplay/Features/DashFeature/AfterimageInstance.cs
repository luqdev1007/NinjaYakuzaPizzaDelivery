using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature
{
    public class AfterimageInstance : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private System.Action<GameObject> _returnToPool;

        public void Initialize(
            Sprite sprite,
            Vector3 position,
            Vector3 scale,
            float lifetime,
            Color startColor,
            System.Action<GameObject> returnToPool)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = startColor;

            transform.position = position;
            transform.localScale = scale;
            _returnToPool = returnToPool;

            _spriteRenderer.DOKill();
            _spriteRenderer.DOFade(0f, lifetime)
                .SetEase(Ease.Linear)
                .OnComplete(() => _returnToPool?.Invoke(gameObject));
        }

        private void OnDestroy()
        {
            _spriteRenderer?.DOKill();
        }
    }
}