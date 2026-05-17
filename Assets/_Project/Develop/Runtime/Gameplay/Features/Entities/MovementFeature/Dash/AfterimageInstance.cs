using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash
{
    public class AfterimageInstance : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _lifetime;
        private float _elapsed;
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
            _lifetime = lifetime;
            _elapsed = 0f;
            _returnToPool = returnToPool;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = _elapsed / _lifetime;

            Color c = _spriteRenderer.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            _spriteRenderer.color = c;

            if (_elapsed >= _lifetime)
                _returnToPool?.Invoke(gameObject);
        }
    }
}