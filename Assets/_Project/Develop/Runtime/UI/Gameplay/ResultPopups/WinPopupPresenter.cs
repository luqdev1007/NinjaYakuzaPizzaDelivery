using Assets._Project.Develop.Runtime.Gameplay.Features.LevelResultsFeature;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels; // Для LevelConfig
using Assets._Project.Develop.Runtime.Gameplay.Features.InGameTimers; // Для таймера
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultPopups
{
    public class WinPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "PIZZA DELIVERED";

        private readonly WinPopupView _view;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        // Новые зависимости для расчетов
        private readonly LevelResultService _levelResultService;
        private readonly LevelConfig _levelConfig;
        private readonly InGameTimerFeatureService _timerService;

        public WinPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            WinPopupView view,
            SceneSwitcherService sceneSwitcherService,
            LevelResultService levelResultService,
            LevelConfig levelConfig,
            InGameTimerFeatureService timerService) : base(coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _view = view;
            _sceneSwitcherService = sceneSwitcherService;

            // Сохраняем сервисы
            _levelResultService = levelResultService;
            _levelConfig = levelConfig;
            _timerService = timerService;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.SetTitle(TitleName);

            float finalTime = 5; //_timerService.ElapsedTime;

            LevelResultReport report = _levelResultService.CalculateResult(_levelConfig, finalTime);

            Debug.Log($"<color=cyan>[WinPopup]</color> Final Time: {finalTime}s (Target: {_levelConfig.TargetTime}s) | Star: {report.TimeStarEarned}");
            Debug.Log($"<color=yellow>[WinPopup]</color> Style Points: {report.FinalStylePoints} (Threshold: {_levelConfig.StyleStarThreshold}) | Rank: {report.StyleLetter} | Star: {report.StyleStarEarned}");
            Debug.Log($"<color=magenta>[WinPopup]</color> Chests: {report.CollectedSecrets}/{report.TotalSecrets} | Star: {report.SecretStarEarned}");

            _view.SetupResults(report);

            _view.ContinueClicked += OnContinueClicked;
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();
            _view.ContinueClicked -= OnContinueClicked;
        }

        public override void Dispose()
        {
            base.Dispose();
            _view.ContinueClicked -= OnContinueClicked;
        }

        private void OnContinueClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu));
            OnCloseRequest();
        }
    }
}