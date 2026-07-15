using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.Bounce;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using UnityEngine;
using DG.Tweening;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InteractiveObjects
{
    /// <summary>
    /// Трамплин остаётся MonoBehaviour осознанно (развилка F5): ручная расстановка на
    /// префабе уровня — рабочий процесс геймдизайнера, упругость тюнится тут же.
    /// </summary>
    /// <remarks>
    /// Но velocity в чужой rigidbody он больше НЕ пишет. Упругость (сколько разогнать)
    /// — его зона ответственности, применение к телу — зона систем самой сущности.
    /// Поэтому наружу уходит уже посчитанная целевая скорость через
    /// BounceImpulseRequest, а применяет её BounceSystem героя.
    ///
    /// Отскок вертикальный: ось всегда Vector2.up, поворот трамплина на направление
    /// не влияет. Наклонных трамплинов в планах уровней нет. Если появятся — их
    /// горизонтальную компоненту придётся защитить lockout'ом по образцу wall-jump
    /// (IsWallJumping + LockoutDuration), иначе базовая локомоция съест боковой
    /// импульс. См. заметку R9 в docs/agent/TIMESTEP_MIGRATION_PLAN.md.
    /// </remarks>
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

            _visualTarget.DOKill(true);
            _visualTarget.DOPunchScale(_punchScale, _animationDuration, _vibrato, _elasticity).SetEase(Ease.OutQuad);
        }
    }
}
