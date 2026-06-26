using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.StyleFeature;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroStyleSystem : IInitializableSystem, IDisposableSystem
    {
        private readonly StyleEvaluator _styleEvaluator;

        private readonly List<IDisposable> _disposables = new();

        public MainHeroStyleSystem(StyleEvaluator styleEvaluator)
        {
            _styleEvaluator = styleEvaluator;
        }

        public void OnInit(Entity entity)
        {
            _disposables.Add(entity.IsDashing.Subscribe(OnDashingChanged));
            _disposables.Add(entity.IsWallJumping.Subscribe(OnWallJumpingChanged));
            _disposables.Add(entity.IsWallHanging.Subscribe(OnWallHangingChanged));

            _disposables.Add(entity.GrappleAnchoredEvent.Subscribe(_styleEvaluator.ProcessGrappleAttach));
            _disposables.Add(entity.SuccessfulHitEvent.Subscribe(OnSuccessfulHit));
            _disposables.Add(entity.PlungeImpactEvent.Subscribe(OnPlungeImpact));

            _disposables.Add(entity.SpeedDamageDealtEvent.Subscribe(OnSpeedDamageDealt));

            _disposables.Add(entity.TakeDamageEvent.Subscribe(OnTakeDamage));
        }

        private void OnDashingChanged(bool wasDashing, bool isDashing)
        {
            if (isDashing)
            {
                _styleEvaluator.ProcessDash();
            }
        }

        private void OnWallJumpingChanged(bool wasWallJumping, bool isWallJumping)
        {
            if (isWallJumping)
            {
                _styleEvaluator.ProcessWallJump();
            }
        }

        private void OnWallHangingChanged(bool wasWallHanging, bool isWallHanging)
        {
            if (isWallHanging)
            {
                _styleEvaluator.ProcessWallHangAttach();
            }
        }

        private void OnSuccessfulHit()
        {
            _styleEvaluator.ProcessHit(StyleActionType.LightAttack, isLethal: false);
        }

        private void OnSpeedDamageDealt()
        {
            _styleEvaluator.ProcessHit(StyleActionType.SpeedDamage, isLethal: false);
        }

        private void OnPlungeImpact(float landVelocity)
        {
            _styleEvaluator.ProcessPlungeSlam();
        }

        private void OnTakeDamage(DamageData damage)
        {
            _styleEvaluator.ProcessPlayerHit();
        }

        public void OnDispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            _disposables.Clear();
        }
    }
}