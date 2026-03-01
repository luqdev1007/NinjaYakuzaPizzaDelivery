using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Endgame
{
    public class WinMenuPopupPresenter : PopupPresenterBase
    {
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly WalletService _walletService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly SceneSwitcherService _sceneSwitcherService;

        private readonly WinMenuPopupView _view;

        public WinMenuPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            WinMenuPopupView view,
            GameplayInputArgs gameplayInputArgs,
            PlayerDataProvider playerDataProvider,
            WalletService walletService,
            SceneSwitcherService sceneSwitcherService) : base(coroutinesPerformer)
        {
            _view = view;

            _coroutinesPerformer = coroutinesPerformer;
            _gameplayInputArgs = gameplayInputArgs;
            _playerDataProvider = playerDataProvider;
            _walletService = walletService;
            _sceneSwitcherService = sceneSwitcherService;

            _view.PillageButtonClicked += OnPillageButtonClicked;
            _view.RestartButtonClicked += OnRestartButtonClicked;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Dispose()
        {
            base.Dispose();

            _view.PillageButtonClicked -= OnPillageButtonClicked;
            _view.RestartButtonClicked -= OnRestartButtonClicked;
        }

        private void OnRestartButtonClicked()
        {
            Debug.Log("Restart!");

            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, _gameplayInputArgs));
        }

        private void OnPillageButtonClicked()
        {
            _coroutinesPerformer.StartPerform(PillageProcess());
        }

        private IEnumerator PillageProcess()
        {
            Debug.Log("Вы ограбили корабль и получили золото!");

            _walletService.Add(CurrencyTypes.Gold, _gameplayInputArgs.LevelConfig.BaseReward * _gameplayInputArgs.LevelConfig.Difficulty);

            yield return _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            yield return _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu, _gameplayInputArgs));
        }
    }
}
