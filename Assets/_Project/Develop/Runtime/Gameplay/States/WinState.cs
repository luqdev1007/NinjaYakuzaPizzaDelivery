using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class WinState : EndGameState, IUpdatableState
    {
        private readonly LevelsProgressionService _levelsProgressionService;
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayPopupService _gameplayPopupService;
        private readonly SessionLootService _sessionLootService;
        private readonly WalletService _walletService;

        public WinState(
            IInputService inputService,
            GameplayScreenPresenter gameplayScreenPresenter,
            LevelsProgressionService levelsProgressionService,
            GameplayInputArgs gameplayInputArgs,
            PlayerDataProvider playerDataProvider,
            ICoroutinesPerformer coroutinesPerformer,
            GameplayPopupService gameplayPopupService,
            SessionLootService sessionLootService,
            WalletService walletService) : base(inputService, gameplayScreenPresenter)
        {
            _levelsProgressionService = levelsProgressionService;
            _gameplayInputArgs = gameplayInputArgs;
            _playerDataProvider = playerDataProvider;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayPopupService = gameplayPopupService;
            _sessionLootService = sessionLootService;
            _walletService = walletService;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("VICTORY!");

            _levelsProgressionService.AddLevelToCompleted(_gameplayInputArgs.LevelNumber);

            foreach (var loot in _sessionLootService.GetAllCollected())
            {
                if (TryMapToCurrency(loot.Key, out CurrencyTypes currencyType))
                {
                    _walletService.Add(currencyType, loot.Value);
                    Debug.Log($"Авто-зачисление: {loot.Value} единиц {currencyType} добавлено в кошелек.");
                }
            }

            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            _gameplayPopupService.OpenWinPopup();

            _sessionLootService.ClearSession();
        }

        public void Update(float deltaTime)
        {
        }

        private bool TryMapToCurrency(LootTypes lootType, out CurrencyTypes currencyType)
        {
            switch (lootType)
            {
                case LootTypes.Coin:
                    currencyType = CurrencyTypes.Coins;
                    return true;

                case LootTypes.SoulShard:
                    currencyType = CurrencyTypes.SoulShard;
                    return true;

                default:
                    currencyType = default;
                    return false;
            }
        }
    }
}