using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Configs.Dialog;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Core.ConfirmPopup;
using Assets._Project.Develop.Runtime.UI.Core.GameSettings;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI.LevelsMenuPopup;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.Timer;
using System;

namespace Assets._Project.Develop.Runtime.UI
{
    public class ProjectPresentersFactory
    {
        private DIContainer _container;

        public ProjectPresentersFactory(DIContainer container)
        {
            _container = container;
        }

        public LevelsMenuPopupPresenter CreateLevelsMenuPopupPresenter(LevelsMenuPopupView view)
        {
            return new LevelsMenuPopupPresenter(
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<ConfigsProviderService>(),
                this,
                _container.Resolve<ViewsFactory>(),
                view
                );
        }

        public LevelTilePresenter CreateLevelTilePresenter(LevelTileView view, GameplayInputArgs inputArgs, LevelConfig config)
        {
            return new LevelTilePresenter(
                _container.Resolve<SceneSwitcherService>(),
                _container.Resolve<ICoroutinesPerformer>(),
                inputArgs,
                view,
                config,
               _container.Resolve<LevelsProgressionService>());
        }

        public CurrencyPresenter CreateCurrencyPresenter(
            IconTextView view, 
            IReadOnlyVariable<int> currency,
            CurrencyTypes currencyType)
        {
            return new CurrencyPresenter(
                currency, 
                currencyType, 
                _container.Resolve<ConfigsProviderService>().GetConfig<CurrencyIconsConfig>(),
                view);
        }

        public AudioSettingsPopupPresenter CreateAudioSettingsPopupPresenter(AudioSettingsPopupView view)
        {
            return new AudioSettingsPopupPresenter(
                view,
                _container.Resolve<AudioService>(),
                _container.Resolve<ICoroutinesPerformer>());
        }

        public WalletPresenter CreateWalletPresenter(IconTextListView view)
        {
            return new WalletPresenter(
                _container.Resolve<WalletService>(),
                this,
                _container.Resolve<ViewsFactory>(),
                view
                );
        }

        public ConfirmPopupPresenter CreateConfirmPopupPresenter(ConfirmPopupView view, Action onConfirmButtonClicked, string header)
        {
            return new ConfirmPopupPresenter(view, _container.Resolve<ICoroutinesPerformer>(), onConfirmButtonClicked, header);
        }

        public GameSettingsPopupPresenter CreateGameSettingsPopupPresenter(GameSettingsPopupView view, PopupService popupService)
        {
            return new GameSettingsPopupPresenter(
                view,
                _container.Resolve<ICoroutinesPerformer>(),
                popupService,
                _container.Resolve<SceneSwitcherService>(),
                _container.Resolve<IInputService>(),
                _container.Resolve<WalletService>()
                );
        }

        public KeyBindingsSettingsPopupPresenter CreateKeyBindingsPopupPresenter(KeyBindingsSettingsPopupView view)
        {
            return new KeyBindingsSettingsPopupPresenter(view, _container.Resolve<ICoroutinesPerformer>());
        }
    }
}
