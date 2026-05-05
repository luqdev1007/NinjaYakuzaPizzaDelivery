using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.DashFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly EntitiesFactory _entitiesFactory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private Entity _entity;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private ICompositeCondition _canStartAttack;
        private Transform _shootPoint;

        // Реактивный инпут из сущности
        private InputState _attackInput;

        private float _chargeTimer;
        private bool _isCharging;
        private bool _isAttackBuffered;

        private const float ChargeThreshold = 0.2f;
        private const float BufferTimeThreshold = 0.15f;
        private const float DoubleAttackDelay = 0.1f;
        private const int DoubleAttackChance = 70;
        private const float ChargedDamageMultiplier = 5f;

        public StartAttackSystem(EntitiesFactory entitiesFactory, ICoroutinesPerformer coroutinesPerformer)
        {
            _entitiesFactory = entitiesFactory;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            _startAttackEvent = entity.StartAttackEvent;
            _inAttackProcess = entity.InAttackProcess;
            _canStartAttack = entity.CanStartAttack;
            _shootPoint = entity.ShootPoint;
            _attackInput = entity.AttackInput;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.IsWallHanging.Value)
            {
                ResetStates();
                return;
            }

            CometDashStateComponent cometState = _entity.CometDashStateC;

            if (_isAttackBuffered && cometState.CooldownTimer.Value <= 0)
            {
                _isAttackBuffered = false;
                ExecuteChargedAttack();
            }

            HandleInputStart();
            HandleCharging(deltaTime);
            HandleInputRelease(cometState);
        }

        private void HandleInputStart()
        {
            if (_attackInput.IsPressed.Value && _canStartAttack.Evaluate() && !_inAttackProcess.Value)
            {
                if (_entity.CanWallHang.Evaluate()) return;

                _isCharging = true;
                _chargeTimer = 0f;
                _isAttackBuffered = false;
            }
        }

        private void HandleCharging(float deltaTime)
        {
            if (!_isCharging) 
                return;

            _chargeTimer += deltaTime;

            /*
            // Если инпут заблокирован (например, станом), сбрасываем зарядку
            if (!_attackInput.IsEnabled.Value)
            {
                _isCharging = false;
            }
            */
        }

        private void HandleInputRelease(CometDashStateComponent cometState)
        {
            if (!_isCharging || !_attackInput.IsReleased.Value) return;

            if (_chargeTimer >= ChargeThreshold)
            {
                if (cometState.CooldownTimer.Value <= BufferTimeThreshold && cometState.CooldownTimer.Value > 0)
                {
                    _isAttackBuffered = true;
                }
                else
                {
                    ExecuteChargedAttack();
                }
            }
            else
            {
                ExecuteNormalAttack();
            }

            _isCharging = false;
            _chargeTimer = 0f;
        }

        private void ExecuteNormalAttack()
        {
            ApplyAttackState();

            if (Random.Range(1, 101) <= DoubleAttackChance)
            {
                _coroutinesPerformer.StartPerform(DoubleAttackRoutine());
            }
        }

        private IEnumerator DoubleAttackRoutine()
        {
            yield return new WaitForSeconds(DoubleAttackDelay);

            if (_entity.InAttackProcess.Value || _entity.CanStartAttack.Evaluate())
                _startAttackEvent.Invoke();
        }

        private void ExecuteChargedAttack()
        {
            CometDashStateComponent cometState = _entity.CometDashStateC;

            if (cometState.CooldownTimer.Value > 0) return;

            ApplyAttackState();

            float finalMultiplier = cometState.CurrentCharges.Value > 0 ? cometState.CurrentMultiplier.Value : 0f;
            float damage = _entity.AttackDamage.Value * ChargedDamageMultiplier * finalMultiplier;
            Vector2 direction = _shootPoint.parent.localScale.x > 0 ? Vector2.right : Vector2.left;

            var projectile = _entitiesFactory.CreateChargedSlashProjectile(_shootPoint, damage, direction, _entity);

            if (finalMultiplier > 0)
            {
                ApplyChargedPhysics(projectile.MoveSpeed.Value, finalMultiplier);
                UpdateCometState(cometState);
            }

            CheckOverheat(cometState);
        }

        private void ApplyAttackState()
        {
            _inAttackProcess.Value = true;
            _startAttackEvent.Invoke();
        }

        private void ApplyChargedPhysics(float moveSpeed, float multiplier)
        {
            float jumpForce = moveSpeed * multiplier;
            _entity.Rigidbody.linearVelocity = new Vector2(_entity.Rigidbody.linearVelocity.x, 0f);
            _entity.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        private void UpdateCometState(CometDashStateComponent state)
        {
            state.CurrentCharges.Value--;
            state.CurrentMultiplier.Value *= state.Config.MultiplierDegradation;
            state.CooldownTimer.Value = state.Config.BaseCooldown;
        }

        private void CheckOverheat(CometDashStateComponent state)
        {
            if (state.CurrentCharges.Value <= 0)
            {
                state.CooldownTimer.Value = state.Config.OverheatCooldown;
                state.CurrentMultiplier.Value = 0f;
            }
        }

        private void ResetStates()
        {
            _isCharging = false;
            _isAttackBuffered = false;
        }
    }
}