using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Endgame
{
    public class DefeatMenuPopupPresenter : PopupPresenterBase
    {
        private readonly GameplayInputArgs _gameplayInputArgs;
        private readonly PlayerDataProvider _playerDataProvider;
        private readonly WalletService _walletService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;
        private readonly SceneSwitcherService _sceneSwitcherService;

        private readonly DefeatMenuPopupView _view;

        public DefeatMenuPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            DefeatMenuPopupView view,
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

            _view.ConcedeButtonClicked += OnConcedeButtonClicked;
            _view.RestartButtonClicked += OnRestartButtonClicked;
            _view.TradeButtonClicked += OnTradeButtonClicked;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Dispose()
        {
            base.Dispose();

            _view.ConcedeButtonClicked -= OnConcedeButtonClicked;
            _view.RestartButtonClicked -= OnRestartButtonClicked;
            _view.TradeButtonClicked -= OnTradeButtonClicked;
        }

        private void OnConcedeButtonClicked()
        {
            _coroutinesPerformer.StartPerform(ConcedeProcess());
        }

        private void OnRestartButtonClicked()
        {
            Debug.Log("Restart!");

            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.Gameplay, _gameplayInputArgs));
        }

        private void OnTradeButtonClicked()
        {
            _coroutinesPerformer.StartPerform(TradeProcess());
        }

        private IEnumerator TradeProcess()
        {
            Debug.Log("вы затрейдили золото за сохранение вашей ничтожной жизни и остатки корабля");

            _walletService.Spend(CurrencyTypes.Gold, _gameplayInputArgs.LevelConfig.BaseReward * _gameplayInputArgs.LevelConfig.Difficulty);

            yield return _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            yield return _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu, _gameplayInputArgs));
        }

        private IEnumerator ConcedeProcess()
        {
            Debug.Log("вы сдались и теряете часть прогресса");

            // _walletService.Add(CurrencyTypes.Gold, _gameplayInputArgs.LevelConfig.BaseReward * _gameplayInputArgs.LevelConfig.Difficulty);

            // yield return _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

            yield return _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu, _gameplayInputArgs));
        }
    }
}
