using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Inventory
{
    public class InventoryView : EntityView
    {
        [SerializeField] private Animator _animator;

        [Header("Audio Events")]
        [SerializeField] private SfxEvent _switchItemSfxConfig;
        [SerializeField] private SfxEvent _throwSfxConfig;

        private static readonly int ThrowTrigger = Animator.StringToHash("Throw");
        private AudioService _audioService;

        protected override void OnEntityStartedWork(Entity entity)
        {
            _audioService = entity.GetComponent<AudioComponent>().Service;

            // Подписка на бросок
            // entity.ThrowEvent.Subscribe(OnThrow);

            // Подписка на смену предмета
            // entity.CurrentThrowableIndex.Subscribe((oldIdx, newIdx) => OnItemSwitched());
        }

        private void OnThrow()
        {
            if (_animator) _animator.SetTrigger(ThrowTrigger);

            // Проигрываем эвент броска
            _audioService?.HandleSFXEvent(_throwSfxConfig);
        }

        private void OnItemSwitched()
        {
            // Проигрываем эвент смены предмета
            _audioService?.HandleSFXEvent(_switchItemSfxConfig);
        }
    }
}