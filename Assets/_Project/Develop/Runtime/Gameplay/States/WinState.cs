using Assets._Project.Develop.Runtime.Configs.Meta.Stats;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{

    public class WinState : EndGameState, IUpdatableState
    {
        private readonly LevelsProgressionService _levelsProgressionService;
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly GameplayPopupService _gameplayPopupService;
        private readonly WalletService _walletService;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly AudioService _audioService;

        public WinState(
            IInputService inputService,
            LevelsProgressionService levelsProgressionService,
            GameplayInputArgs gameplayInputArgs,
            PlayerDataProvider playerDataProvider,
            ICoroutinesPerformer coroutinesPerformer,
            GameplayPopupService gameplayPopupService,
            WalletService walletService,
            ConfigsProviderService configsProviderService,
            AudioService audioService) : base(inputService)
        {
            _levelsProgressionService = levelsProgressionService;
            _gameplayInputArgs = gameplayInputArgs;
            _playerDataProvider = playerDataProvider;
            _coroutinesPerformer = coroutinesPerformer;
            _gameplayPopupService = gameplayPopupService;
            _walletService = walletService;
            _configsProviderService = configsProviderService;
            _audioService = audioService;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("VICTORY!");

            // Обращаемся к AudioService через твой DI (нужно добавить в конструктор)
            _audioService.SetMusicMuted(true);

            int reward = _configsProviderService.GetConfig<GameRewardsConfig>().RewardForWin;
            _walletService.Add(CurrencyTypes.Gold, reward);

            _levelsProgressionService.AddLevelToCompleted(_gameplayInputArgs.LevelNumber);

            _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            _gameplayPopupService.OpenWinPopup();
        }

        public void Update(float deltaTime)
        {
        }
    }
}
