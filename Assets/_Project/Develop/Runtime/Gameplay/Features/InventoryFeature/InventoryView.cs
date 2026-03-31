using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement; // Не забудь импорт
using UnityEngine;

public class InventoryView : EntityView
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _switchItemSfxPrefix = "ItemSwitch"; // Префикс для звука "вжух"
    [SerializeField] private string _throwSfxPrefix = "AbilityImpactHeroThrow";

    private static readonly int ThrowTrigger = Animator.StringToHash("Throw");
    private AudioService _audioService;

    protected override void OnEntityStartedWork(Entity entity)
    {
        _audioService = entity.GetComponent<AudioComponent>().Service;

        // Подписка на бросок
        entity.ThrowEvent.Subscribe(OnThrow);

        // Подписка на смену предмета (пропускаем первое значение при старте через Subscribe)
        entity.CurrentThrowableIndex.Subscribe((oldIdx, newIdx) => OnItemSwitched());
    }

    private void OnThrow()
    {
        _animator.SetTrigger(ThrowTrigger);

        _audioService?.PlaySfxByPrefixAuto(_throwSfxPrefix, Random.Range(0.9f, 1.1f));
    }

    private void OnItemSwitched()
    {
        // Проигрываем звук смены
        _audioService?.PlaySfxByPrefixAuto(_switchItemSfxPrefix, Random.Range(0.95f, 1.05f));
    }
}