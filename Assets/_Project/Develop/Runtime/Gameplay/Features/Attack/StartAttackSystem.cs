using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
    public class StartAttackSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _inputService;
        private readonly EntitiesFactory _entitiesFactory;
        private ReactiveEvent _startAttackEvent;
        private ReactiveVariable<bool> _inAttackProcess;
        private ICompositeCondition _canStartAttack;
        private Transform _shootPoint;
        private Entity _entity;

        // Поля для логики зажима
        private float _chargeTimer;
        private bool _isCharging;
        private const float ChargeThreshold = 0.2f;

        public StartAttackSystem(IInputService inputService, EntitiesFactory entitiesFactory)
        {
            _inputService = inputService;
            _entitiesFactory = entitiesFactory;
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
            // 1. Начало нажатия
            if (_inputService.IsAttackKeyPressed && _canStartAttack.Evaluate() && !_inAttackProcess.Value)
            {
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
        }

        private void ExecuteChargedAttack()
        {
            // Здесь будет спавн прожектайла слэша, а пока просто лог
            Debug.Log("<color=#FFD700><b>[ATTACK]</b></color> Слэш атака (Charged)");

            // Если для слэша тоже нужна анимация взмаха мечом, 
            _inAttackProcess.Value = true;
            _startAttackEvent.Invoke();

            _entitiesFactory.CreateChargedSlashProjectile(
                _shootPoint, 
                damage: _entity.AttackDamage.Value * 5, 
                direction: _shootPoint.parent.localScale.x > 0? Vector2.right : Vector2.left,
                _entity);
        }
    }
}