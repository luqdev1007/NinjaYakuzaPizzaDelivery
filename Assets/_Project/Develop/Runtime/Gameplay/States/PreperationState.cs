using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System.Collections;
using UnityEngine;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.UI.Dialog;
using Assets._Project.Develop.Runtime.UI.Gameplay.Hints;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;

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
        private readonly GameplayPopupService _popupService;
        private readonly LevelConfig _levelConfig;
        private readonly GameplayInputArgs _inputArgs;

        private FreePanBehaviour _panBehaviour;
        private bool _isIntroFinished;
        private bool _dialogFinished;

        private DialogPresenter _activeDialog;
        private HintPresenter _hintPopup;

        public PreperationState(
            StartGameTriggerService startTrigger,
            CameraService cameraService,
            MainHeroFactory mainHeroFactory,
            FinalPointTriggerService finalPoint,
            StageProviderService stageProvider,
            ICoroutinesPerformer coroutines,
            LevelConfig levelConfig,
            GameplayPopupService popupService,
            GameplayInputArgs inputArgs)
        {
            _startTrigger = startTrigger;
            _cameraService = cameraService;
            _mainHeroFactory = mainHeroFactory;
            _finalPoint = finalPoint;
            _stageProvider = stageProvider;
            _coroutines = coroutines;
            _levelConfig = levelConfig;
            _popupService = popupService;
            _inputArgs = inputArgs;
            _panBehaviour = new FreePanBehaviour(25f);
        }

        public override void Enter()
        {
            base.Enter();
            _startTrigger.Reset();
            _stageProvider.PrepareFirstStage();

            if (_inputArgs != null && _inputArgs.IsRestart)
            {
                // Если это рестарт — скипаем всё нафиг
                SkipIntro();
            }
            else
            {
                // Первый вход — играем кино
                _isIntroFinished = false;
                _dialogFinished = _levelConfig.PreparationDialog == null;
                _coroutines.StartPerform(ShowFinishIntro());
            }
        }

        private void SkipIntro()
        {
            _cameraService.SetBehaviour(_panBehaviour);

            // Сразу ставим камеру на финальную точку (или куда нужно)
            Vector3 targetPos = _finalPoint.FinalPointPosition;
            targetPos.z = -10f;
            Camera.main.transform.position = targetPos;

            _dialogFinished = true;
            _isIntroFinished = true;

            ShowHint();
        }

        private IEnumerator ShowFinishIntro()
        {
            _cameraService.SetBehaviour(_panBehaviour);
            yield return null;

            if (_levelConfig.PreparationDialog != null)
            {
                _activeDialog = _popupService.OpenDialog(_levelConfig.PreparationDialog, () =>
                {
                    _dialogFinished = true;
                    _activeDialog = null;
                });
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
            yield return new WaitUntil(() => _dialogFinished);

            ShowHint();
            _isIntroFinished = true;
        }

        private void ShowHint()
        {
            string hintMessage = "Press 'F' to Begin\nUse WASD and LShift to free fly camera";
            _hintPopup = _popupService.OpenHint(hintMessage);
        }

        public void Update(float deltaTime)
        {
            _activeDialog?.Update(deltaTime);

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
            if (_activeDialog != null)
                _popupService.ClosePopup(_activeDialog);

            if (_hintPopup != null)
                _popupService.ClosePopup(_hintPopup);

            base.Exit();

            Entity hero = _mainHeroFactory.Create(_levelConfig.StartPlayerPosition);
            _stageProvider.StartCurrent();
            _cameraService.SetBehaviour(new FollowBehaviour(hero.Transform, new Vector3(0, 2, -10)));
        }
    }
}