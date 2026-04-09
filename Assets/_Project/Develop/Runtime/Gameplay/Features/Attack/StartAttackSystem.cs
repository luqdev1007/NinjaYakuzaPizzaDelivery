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
                return;
            }

            if (_inputService.IsAttackKeyPressed && _canStartAttack.Evaluate() && !_inAttackProcess.Value)
            {
                if (_entity.CanWallHang.Evaluate()) return;

                _isCharging = true;
                _chargeTimer = 0f;
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

            if (_isCharging && _inputService.IsAttackKeyReleased)
            {
                if (_chargeTimer >= ChargeThreshold)
                    ExecuteChargedAttack();
                else
                    ExecuteNormalAttack();

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

            // Если висит кулдаун (перегрев или базовый), мы НЕ можем использовать усиленную атаку
            if (state.CooldownTimer.Value > 0)
            {
                Debug.Log("<color=red>[ATTACK]</color> Способность перегрета!");
                return;
            }

            _inAttackProcess.Value = true;
            _startAttackEvent.Invoke();

            // Рассчитываем множитель: если заряды кончились, множитель 0 (нет прыжка)
            float finalMultiplier = state.CurrentCharges.Value > 0 ? state.CurrentMultiplier.Value : 0f;

            var projectile = _entitiesFactory.CreateChargedSlashProjectile(
                _shootPoint,
                damage: _entity.AttackDamage.Value * 5 * finalMultiplier,
                direction: _shootPoint.parent.localScale.x > 0 ? Vector2.right : Vector2.left,
                _entity);

            // Прыжок будет только если есть заряды
            if (finalMultiplier > 0)
            {
                float jumpForce = projectile.MoveSpeed.Value * finalMultiplier;
                _entity.Rigidbody.linearVelocity = new Vector2(_entity.Rigidbody.linearVelocity.x, 0f);
                _entity.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

                // Тратим заряд и деградируем множитель только при успешном прыжке
                state.CurrentCharges.Value--;
                state.CurrentMultiplier.Value *= state.Config.MultiplierDegradation;

                // Ставим базовый КД между выстрелами
                state.CooldownTimer.Value = state.Config.BaseCooldown;
            }

            // Если после выстрела заряды ушли в ноль — включаем Overheat
            if (state.CurrentCharges.Value <= 0)
            {
                state.CooldownTimer.Value = state.Config.OverheatCooldown;
                state.CurrentMultiplier.Value = 0f; // Обнуляем силу для следующей попытки до восстановления
                Debug.Log("<color=orange>[ATTACK]</color> OVERHEAT! Ждем восстановления ресурсов.");
            }
        }
    }
}