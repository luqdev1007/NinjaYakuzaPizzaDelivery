using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections;
using System.Collections.Generic;
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

        // Поля для логики зажима
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
            // 0. Если мы уже висим на стене — игнорируем логику атаки/зарядки
            if (_entity.IsWallHanging.Value)
            {
                _isCharging = false;
                return;
            }

            // 1. Начало нажатия
            // Добавляем проверку: НЕ начинаем зарядку, если прямо сейчас можем зацепиться за стену
            if (_inputService.IsAttackKeyPressed && _canStartAttack.Evaluate() && !_inAttackProcess.Value)
            {
                // Если WallHangSystem разрешает вис, то атаку не копим
                if (_entity.CanWallHang.Evaluate()) return;

                _isCharging = true;
                _chargeTimer = 0f;
            }

            // 2. Процесс удержания
            if (_isCharging)
            {
                _chargeTimer += deltaTime;

                // Страховка: если ввод заблокировали (например, открыли меню), сбрасываем зарядку
                if (!_inputService.IsEnabled)
                {
                    _isCharging = false;
                    return;
                }
            }

            // 3. Момент отпускания кнопки
            if (_isCharging && _inputService.IsAttackKeyReleased)
            {
                if (_chargeTimer >= ChargeThreshold)
                {
                    // Удерживали дольше 0.2 сек
                    ExecuteChargedAttack();
                }
                else
                {
                    // Быстрый клик
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
            Debug.Log("<color=white>[ATTACK]</color> Normal Attack");

            // Рандомим двойную атаку (например, шанс 70%)
            if (Random.Range(1, 101) <= 70)
            {
                // Используем встроенный в Unity или твой самописный таймер для задержки
                // Чтобы вторая тычка вылетела через 0.1 - 0.2 сек
                _coroutinesPerformer.StartPerform(DoubleAttackRoutine());
            }
        }

        private IEnumerator DoubleAttackRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            _startAttackEvent.Invoke();
            Debug.Log("<color=cyan>[ATTACK]</color> DOUBLE PROC!");
        }

        private void ExecuteChargedAttack()
        {
            var state = _entity.CometDashStateC;

            // Блокируем атаку, если кулдаун еще не прошел
            if (state.CooldownTimer.Value > 0)
            {
                Debug.Log("<color=red>[ATTACK]</color> Слэш на перезарядке!");
                return;
            }

            Debug.Log("<color=#FFD700><b>[ATTACK]</b></color> Слэш атака (Charged)");

            _inAttackProcess.Value = true;
            _startAttackEvent.Invoke();

            // Спавним снаряд
            var projectile = _entitiesFactory.CreateChargedSlashProjectile(
                _shootPoint,
                damage: _entity.AttackDamage.Value * 5 * state.CurrentMultiplier.Value,
                direction: _shootPoint.parent.localScale.x > 0 ? Vector2.right : Vector2.left,
                _entity);

            // Применяем текущий коэффициент к силе подброса
            float jumpForce = projectile.MoveSpeed.Value * state.CurrentMultiplier.Value;
            // _entity.Rigidbody.linearVelocity = Vector2.zero;
            _entity.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Уменьшаем множитель для следующего раза
            state.CurrentMultiplier.Value *= state.Config.MultiplierDegradation;

            // Устанавливаем кулдаун. Чем ниже упал множитель, тем дольше ждать (штраф за спам)
            state.CooldownTimer.Value = state.Config.BaseCooldown;

            // state.CurrentCharges.Value--;
        }
    }
}