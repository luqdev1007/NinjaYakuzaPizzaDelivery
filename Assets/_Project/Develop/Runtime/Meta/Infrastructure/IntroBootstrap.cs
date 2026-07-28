using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class IntroBootstrap : SceneBootstrap
    {
        // mirror of DialogPresenter.SkipHoldDuration, unify later
        private const float SkipHoldDuration = 0.6f;

        [SerializeField] private PlayableDirector _director;

        private DIContainer _container;
        private IInputService _inputService;
        private PlayerDataProvider _playerDataProvider;
        private SceneSwitcherService _sceneSwitcherService;
        private ICoroutinesPerformer _coroutinesPerformer;

        private float _holdTime;
        private bool _isHolding;
        private bool _isPlaying;
        private bool _isFinished;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
        }

        public override IEnumerator Initialize()
        {
            if (_director == null)
                throw new NullReferenceException($"{nameof(PlayableDirector)} is not assigned on {nameof(IntroBootstrap)}");

            _inputService = _container.Resolve<IInputService>();
            _playerDataProvider = _container.Resolve<PlayerDataProvider>();
            _sceneSwitcherService = _container.Resolve<SceneSwitcherService>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();

            // IInputService — синглтон project-скоупа, общий на всю сессию. После
            // жёсткого teardown сцены он может остаться выключенным, и тогда скип
            // молча не сработает (правило #4 из CLAUDE.md). Выставляем явно.
            _inputService.IsEnabled = true;

            _director.playOnAwake = false;

            // None: при Hold/Loop директор после конца тайплайна остаётся играющим
            // и события stopped не будет — интро повиснет без перехода в меню.
            _director.extrapolationMode = DirectorWrapMode.None;

            // Unscaled: Time.timeScale может прийти грязным после hit-stop'а
            // геймплея. Диалоговые твины по той же причине живут на SetUpdate(true).
            _director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;

            _director.stopped += OnDirectorStopped;

            yield break;
        }

        public override void Run()
        {
            _isPlaying = true;
            _director.Play();
        }

        private void Update()
        {
            if (_isPlaying == false || _isFinished)
                return;

            HandleSkipInput(Time.unscaledDeltaTime);
        }

        // Зеркалит DialogPresenter.HandleInput, но без ветки "короткое нажатие
        // промотает строку" — в интро мотать нечего, скип только по удержанию.
        // Стартовать накопление обязательно с edge (IsInteractKeyPressed), иначе
        // зажатая E, которой игрок закрыл "Press Any Key", скипнет интро сразу.
        private void HandleSkipInput(float deltaTime)
        {
            if (_inputService.IsInteractKeyPressed)
            {
                _isHolding = true;
                _holdTime = 0f;
            }

            if (_isHolding && _inputService.IsInteractKeyHeld)
            {
                _holdTime += deltaTime;

                if (_holdTime >= SkipHoldDuration)
                {
                    _isHolding = false;
                    _director.Stop();
                    return;
                }
            }

            if (_inputService.IsInteractKeyReleased)
            {
                _isHolding = false;
                _holdTime = 0f;
            }
        }

        private void OnDirectorStopped(PlayableDirector director)
        {
            // Stop() и естественный конец тайплайна оба шлют stopped — отрабатываем
            // ровно один раз.
            if (_isFinished)
                return;

            _isFinished = true;
            _isPlaying = false;

            _director.stopped -= OnDirectorStopped;

            // Корутина живёт на CoroutinesPerformer (DontDestroyOnLoad) и потому
            // переживает выгрузку этой сцены внутри ProcessingSwitchTo.
            _coroutinesPerformer.StartPerform(FinishIntro());
        }

        private IEnumerator FinishIntro()
        {
            _playerDataProvider.IntroSeen = true;

            yield return _playerDataProvider.SaveAsync();

            yield return _sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu);
        }

        private void OnDestroy()
        {
            if (_director != null)
                _director.stopped -= OnDirectorStopped;
        }
    }
}
