using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
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
        private readonly IInputService _inputService;
        private readonly EntitiesFactory _entitiesFactory;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private ICompositeCondition _canStartAttack;
        private Transform _shootPoint;
        private Entity _entity;

        private float _chargeTimer;
        private bool _isCharging;
        private const float ChargeThreshold = 0.2f;

        // Поля для Coyote Time / Buffering
        private bool _isAttackBuffered;
        private const float BufferTimeThreshold = 0.15f;

        public StartAttackSystem(IInputService inputService, EntitiesFactory entitiesFactory, ICoroutinesPerformer coroutinesPerformer)
        {
            _inputService = inputService;
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
        }

        public void OnUpdate(float deltaTime)
        {
            if (_entity.IsWallHanging.Value)
            {
                _isCharging = false;
                _isAttackBuffered = false;
                return;
            }

            var state = _entity.CometDashStateC;

            // 1. Обработка буферизированной атаки
            if (_isAttackBuffered && state.CooldownTimer.Value <= 0)
            {
                _isAttackBuffered = false;
                ExecuteChargedAttack();
            }

            // 2. Начало нажатия
            if (_inputService.IsAttackKeyPressed && _canStartAttack.Evaluate() && !_inAttackProcess.Value)
            {
                if (_entity.CanWallHang.Evaluate()) return;

                _isCharging = true;
                _chargeTimer = 0f;
                _isAttackBuffered = false;
            }

            if (_isCharging)
            {
                _chargeTimer += deltaTime;
                if (!_inputService.IsEnabled)
                {
                    _isCharging = false;
                    return;
                }
            }

            // 3. Отпускание кнопки
            if (_isCharging && _inputService.IsAttackKeyReleased)
            {
                if (_chargeTimer >= ChargeThreshold)
                {
                    // Проверяем: можно ли выстрелить сейчас или нужно закинуть в буфер?
                    if (state.CooldownTimer.Value <= BufferTimeThreshold && state.CooldownTimer.Value > 0)
                    {
                        _isAttackBuffered = true;
                        Debug.Log("<color=cyan>[BUFFER]</color> Attack buffered");
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
        }

        private void ExecuteNormalAttack()
        {
            _inAttackProcess.Value = true;
            _startAttackEvent.Invoke();
            if (Random.Range(1, 101) <= 70)
                _coroutinesPerformer.StartPerform(DoubleAttackRoutine());
        }

        private IEnumerator DoubleAttackRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            _startAttackEvent.Invoke();
        }

        private void ExecuteChargedAttack()
        {
            var state = _entity.CometDashStateC;

            if (state.CooldownTimer.Value > 0)
            {
                // Если мы попали сюда не через буфер, а просто спамом
                return;
            }

            _inAttackProcess.Value = true;
            _startAttackEvent.Invoke();

            float finalMultiplier = state.CurrentCharges.Value > 0 ? state.CurrentMultiplier.Value : 0f;

            var projectile = _entitiesFactory.CreateChargedSlashProjectile(
                _shootPoint,
                damage: _entity.AttackDamage.Value * 5 * finalMultiplier,
                direction: _shootPoint.parent.localScale.x > 0 ? Vector2.right : Vector2.left,
                _entity);

            if (finalMultiplier > 0)
            {
                float jumpForce = projectile.MoveSpeed.Value * finalMultiplier;
                _entity.Rigidbody.linearVelocity = new Vector2(_entity.Rigidbody.linearVelocity.x, 0f);
                _entity.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                state.CurrentCharges.Value--;
                state.CurrentMultiplier.Value *= state.Config.MultiplierDegradation;
                state.CooldownTimer.Value = state.Config.BaseCooldown;
            }

            if (state.CurrentCharges.Value <= 0)
            {
                state.CooldownTimer.Value = state.Config.OverheatCooldown;
                state.CurrentMultiplier.Value = 0f;
                Debug.Log("<color=orange>[ATTACK]</color> OVERHEAT!");
            }
        }
    }
}