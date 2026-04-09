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
                SkipIntro();
            }
            else
            {
                _isIntroFinished = false;
                _dialogFinished = _levelConfig.PreparationDialog == null;
                _coroutines.StartPerform(ShowFinishIntro());
            }
        }

        private void SkipIntro()
        {
            _cameraService.SetBehaviour(_panBehaviour);

            Vector3 targetPos = _finalPoint.FinalPointPosition;
            targetPos.z = -10f;
            Camera.main.transform.position = targetPos;

            _dialogFinished = true;
            _isIntroFinished = true;

            ShowHint();
        }

        private IEnumerator ShowFinishIntro()
        {
            // 1. Даем команду камере лететь к финишу с зумом 14 (чтобы увидеть масштаб)
            _cameraService.ShowTargetTemporarily(_finalPoint.FinalPointPosition, 14f);

            // 2. Параллельно может идти диалог
            if (_levelConfig.PreparationDialog != null)
            {
                _activeDialog = _popupService.OpenDialog(_levelConfig.PreparationDialog, () =>
                {
                    _dialogFinished = true;
                    _activeDialog = null;
                });
            }

            // Ждем, пока камера долетит и диалог кончится
            // yield return new WaitForSeconds(2.5f);
            yield return new WaitUntil(() => Vector2.Distance(Camera.main.transform.position, _finalPoint.FinalPointPosition) < 0.5f);
            yield return new WaitUntil(() => _dialogFinished);

            // 3. Возвращаем камеру в режим свободного панорамирования (PrepState это любит)
            _cameraService.StopShowingTarget();
            _cameraService.SetZoom(10f);

            _cameraService.SetBehaviour(_panBehaviour);

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

            // 1. Сначала принудительно выключаем временный показ цели, если он завис
            _cameraService.StopShowingTarget();

            // 2. Устанавливаем слежку за героем
            _cameraService.SetBehaviour(new FollowBehaviour(hero.Transform, new Vector3(0, 2, -10)));

            // 3. ВОЗВРАЩАЕМ ЗУМ. 
            // Если в CameraService есть метод ResetZoom() — используй его.
            // Если нет, можно сделать прямо через Camera.main или прокинуть значение в сервис.
            // Предположим, стандартный зум у тебя 7f:
            _cameraService.SetZoom(10f);
        }
    }
}