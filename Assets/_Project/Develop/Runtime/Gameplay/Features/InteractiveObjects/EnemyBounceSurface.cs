using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InteractiveObjects
{
    /// <summary>
    /// Батут на теле врага: верхняя площадка (узел Jumpable) отскакивает героя вверх.
    /// MonoBehaviour по тем же причинам, что и <see cref="Trampoline"/> — упругость
    /// тюнится прямо на префабе врага, это зона геймдизайнера.
    /// </summary>
    /// <remarks>
    /// Висит на КОРНЕ префаба, а не на узле площадки: Unity доставляет
    /// OnCollisionEnter2D на GameObject с Rigidbody2D, а тот у врага на корне.
    /// Дочерний коллайдер площадки колбэк сам не получает, поэтому нужный
    /// коллайдер отфильтровывается вручную через collision.otherCollider —
    /// иначе удар в бок по корневой капсуле тоже давал бы отскок.
    ///
    /// В чужой rigidbody velocity не пишет: наружу уходит только посчитанная
    /// целевая скорость через BounceImpulseRequest, применяет её BounceSystem
    /// самой сущности героя. Та же развилка ответственности, что в Trampoline.
    ///
    /// Отскок вертикальный: ось всегда Vector2.up, поворот врага игнорируется.
    /// </remarks>
    public class EnemyBounceSurface : MonoBehaviour
    {
        [Header("Площадка отскока")]
        [Tooltip("Коллайдер узла Jumpable. Отскок даёт только он, корневая капсула — нет.")]
        [SerializeField] private Collider2D _bounceCollider;

        [Header("Настройки упругости")]
        [SerializeField] private float _baseBounceForce = 14f;
        [SerializeField] private float _bouncinessMultiplier = 1.3f;
        [SerializeField] private float _maxBounceForce = 32f;

        [Header("Настройки DOTween Анимации")]
        [Tooltip("Визуальный узел (SlimeView). Намеренно НЕ корень: пунш корня масштабировал бы коллайдеры и rigidbody вместе с ними.")]
        [SerializeField] private Transform _visualTarget;
        [SerializeField] private Vector3 _punchScale = new Vector3(0.4f, -0.5f, 0f);
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private int _vibrato = 12;
        [SerializeField] private float _elasticity = 1f;

        private MonoEntity _ownerMonoEntity;

        private void Awake()
        {
            _ownerMonoEntity = GetComponent<MonoEntity>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_bounceCollider == null)
                return;

            if (collision.otherCollider != _bounceCollider)
                return;

            if (IsOwnerDead())
                return;

            if (collision.gameObject.TryGetComponent(out MonoEntity monoEntity) == false)
                return;

            Entity entity = monoEntity.LinkedEntity;

            if (entity == null || entity.HasComponent<IsMainHero>() == false)
                return;

            float incomingUpVelocity = Vector2.Dot(collision.relativeVelocity, Vector2.up);

            if (incomingUpVelocity >= -0.1f)
                return;

            float impactSpeed = -incomingUpVelocity;
            float finalLaunchVelocity = _baseBounceForce + (impactSpeed * _bouncinessMultiplier);
            finalLaunchVelocity = Mathf.Min(finalLaunchVelocity, _maxBounceForce);

            entity.BounceImpulseRequest.Invoke(new BounceImpulseData
            {
                UpAxis = Vector2.up,
                LaunchVelocity = finalLaunchVelocity,
            });

            PlayPunch();
        }

        /// <summary>
        /// LinkedEntity проставляется в MonoEntity.Link уже после Awake, поэтому на
        /// первых кадрах он законно null — это не ошибка, просто отскока ещё нет.
        /// </summary>
        private bool IsOwnerDead()
        {
            if (_ownerMonoEntity == null)
                return true;

            Entity ownerEntity = _ownerMonoEntity.LinkedEntity;

            if (ownerEntity == null)
                return true;

            if (ownerEntity.TryGetIsDead(out ReactiveVariable<bool> isDead) == false)
                return false;

            return isDead.Value;
        }

        private void PlayPunch()
        {
            if (_visualTarget == null)
                return;

            _visualTarget.DOKill(true);
            _visualTarget.DOPunchScale(_punchScale, _animationDuration, _vibrato, _elasticity).SetEase(Ease.OutQuad);
        }
    }
}
