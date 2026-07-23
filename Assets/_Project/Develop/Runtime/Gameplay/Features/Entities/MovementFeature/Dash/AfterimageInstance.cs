using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Dash
{
    /// <summary>
    /// Одна копия-афтеримидж. Используется двумя вьюхами с РАЗНЫМ характером:
    /// DashView оставляет статичные копии вдоль траектории рывка, AfterimageView
    /// разбрасывает их веером вокруг стоящего героя при уклонении.
    ///
    /// Поэтому снос, рост масштаба, порядок отрисовки и стартовая альфа —
    /// НЕОБЯЗАТЕЛЬНЫЕ параметры со значениями по умолчанию, которые в точности
    /// воспроизводят прежнее поведение (нет сноса, нет роста, порядок из
    /// префаба, полная непрозрачность). Вызов из DashView остался шестиаргументным
    /// и работает ровно как раньше.
    /// </summary>
    public class AfterimageInstance : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _lifetime;
        private float _elapsed;
        private System.Action<GameObject> _returnToPool;

        private Vector2 _drift;
        private float _scaleGrowth;
        private float _startAlpha;
        private Vector3 _baseScale;

        public void Initialize(
            Sprite sprite,
            Vector3 position,
            Vector3 scale,
            float lifetime,
            Color startColor,
            System.Action<GameObject> returnToPool,
            Vector2 drift = default,
            float scaleGrowth = 0f,
            int? sortingOrder = null,
            float startAlpha = 1f)
        {
            _spriteRenderer.sprite = sprite;
            _spriteRenderer.color = startColor;
            transform.position = position;
            transform.localScale = scale;
            _lifetime = lifetime;
            _elapsed = 0f;
            _returnToPool = returnToPool;

            _drift = drift;
            _scaleGrowth = scaleGrowth;
            _startAlpha = startAlpha;
            _baseScale = scale;

            // null — оставить порядок из префаба. Нужно потому, что у следа рывка
            // порядок 0 (позади героя, там это и правильно: копии остаются за
            // спиной), а копиям уклонения приходится перекрывать героя, иначе на
            // неподвижном герое их не видно вовсе.
            if (sortingOrder.HasValue)
            {
                _spriteRenderer.sortingOrder = sortingOrder.Value;
            }
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = _elapsed / _lifetime;

            Color c = _spriteRenderer.color;
            c.a = Mathf.Lerp(_startAlpha, 0f, t);
            _spriteRenderer.color = c;

            // Снос затухающий: множитель (1 - t) даёт ease-out, копия резко
            // отходит от героя и замирает, растворяясь. Равномерный снос читался
            // бы как полёт объекта, а не как расплывающийся силуэт.
            if (_drift != Vector2.zero)
            {
                transform.position += (Vector3)(_drift * ((1f - t) * Time.deltaTime));
            }

            if (_scaleGrowth != 0f)
            {
                transform.localScale = _baseScale * (1f + _scaleGrowth * t);
            }

            if (_elapsed >= _lifetime)
                _returnToPool?.Invoke(gameObject);
        }
    }
}
