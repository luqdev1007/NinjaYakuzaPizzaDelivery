using Assets._Project.Develop.Runtime.Configs.Inventory;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Projectiles;
using Assets._Project.Develop.Runtime.Configs.Inventory.Potions;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using System; // Добавлено для IDisposable
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventoryView : EntityView, IRequireAudioService
    {
        private static readonly int ThrowTrigger = Animator.StringToHash("Throw");
        private static readonly int DrinkTrigger = Animator.StringToHash("Drink");

        [SerializeField] private Animator _animator;

        [Header("SFX Keys")]
        [SerializeField] private string _emptyUseSfxKey = "ItemEmpty";

        private Entity _entity;
        private IAudioService _audioService;

        private IDisposable _itemUsedDisposable;
        private IDisposable _itemIntentDisposable; // Подписка на попытку нажатия

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        protected override void OnEntityStartedWork(Entity entity)
        {
            _entity = entity;

            // На успешное использование — ТОЛЬКО анимации
            _itemUsedDisposable = _entity.ItemUsedEvent.Subscribe(OnItemUsed);

            // На саму попытку нажать кнопку (замени UseItemIntent на то, как у тебя называется поток инпута в Entity)
            _itemIntentDisposable = _entity.IntentUseItem.Subscribe(OnUseAttempt);
        }

        // Срабатывает КАЖДЫЙ раз, когда игрок жмет кнопку "Использовать"
        private void OnUseAttempt(bool oldValue, bool newValue)
        {
            if (_entity == null && newValue == false) 
                return;

            int currentIndex = _entity.CurrentItemIndex.Value;
            var chargesList = _entity.InventoryCharges;

            if (chargesList != null && currentIndex >= 0 && currentIndex < chargesList.Count)
            {
                // Если зарядов нет — спамим звуком пустой корзины на каждый клик
                if (chargesList[currentIndex].Value <= 0)
                {
                    _audioService?.PlaySfx(_emptyUseSfxKey, transform.position);
                }
            }
        }

        // Срабатывает ТОЛЬКО когда система разрешила использование и списала заряд
        private void OnItemUsed(InventoryItemConfig config)
        {
            if (_animator == null) return;

            // Здесь больше нет проверок на 0, так что анимация последнего патрона/зелья отработает честно
            if (config is ThrowableItemConfig)
            {
                _animator.SetTrigger(ThrowTrigger);
            }
            else if (config is PotionItemConfig)
            {
                // _animator.SetTrigger(DrinkTrigger);
            }
        }

        public override void Cleanup(Entity entity)
        {
            base.Cleanup(entity);
            _itemUsedDisposable?.Dispose();
            _itemIntentDisposable?.Dispose();
        }
    }
}