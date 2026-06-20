using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InteractiveObjects
{
    public class Trampoline : MonoBehaviour
    {
        [Header("Настройки упругости")]
        [SerializeField] private float _baseBounceForce = 5f;
        [SerializeField] private float _bouncinessMultiplier = 1.5f;
        [SerializeField] private float _maxBounceForce = 50f;

        [Header("Настройки DOTween Анимации")]
        [SerializeField] private Transform _visualTarget;
        [SerializeField] private Vector3 _punchScale = new Vector3(0.4f, -0.5f, 0f);
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private int _vibrato = 12;
        [SerializeField] private float _elasticity = 1f;

        private void Start()
        {
            if (_visualTarget == null)
            {
                _visualTarget = transform;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out MonoEntity entity) == false
                || entity.LinkedEntity.HasComponent<IsMainHero>() == false)
                return;

            Rigidbody2D rb = entity.LinkedEntity.Rigidbody;
            if (rb == null)
                return;

            float incomingUpVelocity = Vector2.Dot(collision.relativeVelocity, transform.up);

            if (incomingUpVelocity < -0.1f)
            {
                float impactSpeed = -incomingUpVelocity;
                float finalLaunchVelocity = _baseBounceForce + (impactSpeed * _bouncinessMultiplier);
                finalLaunchVelocity = Mathf.Min(finalLaunchVelocity, _maxBounceForce);

                float currentUpVelocity = Vector2.Dot(rb.linearVelocity, transform.up);
                rb.linearVelocity -= (Vector2)transform.up * currentUpVelocity;

                rb.AddForce(transform.up * finalLaunchVelocity * rb.mass, ForceMode2D.Impulse);

                _visualTarget.DOKill(true);
                _visualTarget.DOPunchScale(_punchScale, _animationDuration, _vibrato, _elasticity).SetEase(Ease.OutQuad);
            }
        }
    }
}