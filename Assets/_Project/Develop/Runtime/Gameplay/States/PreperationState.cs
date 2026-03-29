using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Core.ConfirmPopup;
using Assets._Project.Develop.Runtime.UI.Gameplay.Hints;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class PreperationState : State, IUpdatableState
    {
        private readonly StartGameTriggerService _startTrigger;
        private readonly CameraService _cameraService;
        private readonly MainHeroFactory _mainHeroFactory;
        private readonly FinalPointTriggerService _finalPoint;
        private readonly StageProviderService _stageProvider;
        private readonly ICoroutinesPerformer _coroutines;
        private readonly GameplayUIRoot _gameplayUIRoot;
        private readonly GameplayPopupService _popupService; // Добавлено

        private FreePanBehaviour _panBehaviour;
        private bool _isIntroFinished;

        private readonly ProjectPresentersFactory _presentersFactory;
        private readonly LevelConfig _levelConfig;
        private readonly ViewsFactory _viewsFactory;

        private DialogPresenter _activeDialogPresenter;
        private HintPresenter _hintPopup; // Ссылка на попап с подсказкой
        private bool _dialogFinished = false;

        public PreperationState(
            StartGameTriggerService startTrigger,
            CameraService cameraService,
            MainHeroFactory mainHeroFactory,
            FinalPointTriggerService finalPoint,
            StageProviderService stageProvider,
            ICoroutinesPerformer coroutines,
            ProjectPresentersFactory presentersFactory,
            LevelConfig levelConfig,
            ViewsFactory viewsFactory,
            GameplayUIRoot gameplayUIRoot,
            GameplayPopupService popupService) // Инжектим сервис
        {
            _startTrigger = startTrigger;
            _cameraService = cameraService;
            _mainHeroFactory = mainHeroFactory;
            _finalPoint = finalPoint;
            _stageProvider = stageProvider;
            _coroutines = coroutines;
            _panBehaviour = new FreePanBehaviour(25f);
            _presentersFactory = presentersFactory;
            _levelConfig = levelConfig;
            _viewsFactory = viewsFactory;
            _gameplayUIRoot = gameplayUIRoot;
            _popupService = popupService;
        }

        public override void Enter()
        {
            base.Enter();
            _startTrigger.Reset();
            _isIntroFinished = false;
            _dialogFinished = _levelConfig.PreparationDialog == null; 

            _stageProvider.PrepareFirstStage();
            _coroutines.StartPerform(ShowFinishIntro());
        }

        private IEnumerator ShowFinishIntro()
        {
            _cameraService.SetBehaviour(_panBehaviour);
            yield return null;

            if (_levelConfig.PreparationDialog != null)
            {
                _coroutines.StartPerform(StartDialogSequence());
            }

            Vector3 targetPos = _finalPoint.FinalPointPosition;
            targetPos.z = -10f;

            float duration = 2f;
            float elapsed = 0f;
            Vector3 startPos = Camera.main.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);
                Camera.main.transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.8f);

            // ПОКАЗЫВАЕМ ПОДСКАЗКУ ПОСЛЕ ОБЛЕТА
            string hintMessage = "Press 'F' to Begin\nUse WASD and LShift to free fly camera";
            _hintPopup = _popupService.OpenHint(hintMessage);

            _isIntroFinished = true;
        }

        private IEnumerator StartDialogSequence()
        {
            var dialogView = _viewsFactory.Create<DialogDisplayView>(ViewIDs.DialogDisplayView, _gameplayUIRoot.PopupsLayer);
            _activeDialogPresenter = _presentersFactory.CreateDialogPresenter(dialogView, _levelConfig.PreparationDialog);
            _activeDialogPresenter.DialogEnded += OnDialogEnded;
            _activeDialogPresenter.Initialize();

            yield return new WaitUntil(() => _dialogFinished);

            _activeDialogPresenter.Dispose();
            _activeDialogPresenter = null;
        }

        private void OnDialogEnded()
        {
            _dialogFinished = true;
        }

        public void Update(float deltaTime)
        {
            _activeDialogPresenter?.Update(deltaTime);

            if (!_isIntroFinished)
                return;

            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            _panBehaviour.SetInput(new Vector2(x, y));

            if (Input.GetKeyDown(KeyCode.F) && _dialogFinished)
            {
                if (_hintPopup != null)
                {
                    _popupService.ClosePopup(_hintPopup);
                    _hintPopup = null;
                }

                _startTrigger.RequestStart();
            }
        }

        public override void Exit()
        {
            if (_hintPopup != null)
            {
                _popupService.ClosePopup(_hintPopup);
                _hintPopup = null;
            }

            base.Exit();

            Entity hero = _mainHeroFactory.Create(_levelConfig.StartPlayerPosition);
            _stageProvider.StartCurrent();
            _cameraService.SetBehaviour(new FollowBehaviour(hero.Transform, new Vector3(0, 2, -10)));
        }
    }
}