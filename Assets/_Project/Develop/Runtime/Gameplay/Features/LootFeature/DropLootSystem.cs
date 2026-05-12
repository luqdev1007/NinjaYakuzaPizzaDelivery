using Assets._Project.Develop.Runtime.Configs.Gameplay.Loot;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Conditions;
using Assets._Project.Develop.Runtime.Utilites.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{

    public class DropLootSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly DropLootService _dropLootService;
        private readonly LootTableConfig _lootTable;

        private ICompositeCondition _dropLootCondition;
        private ReactiveVariable<bool> _lootIsDropped;
        private Entity _entity;

        public DropLootSystem(
            DropLootService dropLootService,
            LootTableConfig lootTable)
        {
            _dropLootService = dropLootService;
            _lootTable = lootTable;
        }

        public void OnInit(Entity entity)
        {
            _entity = entity;
            // _lootIsDropped = entity.LootIsDropped;
            // _dropLootCondition = entity.CanDropLoot;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_lootIsDropped.Value == false && _dropLootCondition.Evaluate())
            {
                _dropLootService.DropLootFor(_entity, _lootTable);
                _lootIsDropped.Value = true;
            }
        }
    }
}