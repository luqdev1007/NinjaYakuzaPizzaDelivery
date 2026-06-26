using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.Hints;
using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Utilities.SceneManagement
{
    public class SceneSwitcherService
    {
        private readonly SceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly DIContainer _projectContainer;
        private readonly ICoroutinesPerformer _coroutines;
        private readonly ILoadingHintsService _loadingHintsService;

        private DIContainer _currentSceneContainer;

        public SceneSwitcherService(
            SceneLoaderService sceneLoaderService,
            ILoadingScreen loadingScreen,
            DIContainer projectContainer,
            ICoroutinesPerformer coroutines,
            ILoadingHintsService loadingHintsService)
        {
            _sceneLoaderService = sceneLoaderService;
            _loadingScreen = loadingScreen;
            _projectContainer = projectContainer;
            _coroutines = coroutines;
            _loadingHintsService = loadingHintsService;
        }

        public IEnumerator ProcessingSwitchTo(string sceneName, IInputSceneArgs sceneArgs = null)
        {
            CoolLoadingScreen coolScreen = _loadingScreen as CoolLoadingScreen;
            Coroutine hintRoutine = _coroutines.StartPerform(HintCycleRoutine(coolScreen));

            try
            {
                _loadingScreen.Show();

                _currentSceneContainer?.Dispose();

                yield return _sceneLoaderService.LoadAsync(Scenes.Empty);
                yield return _sceneLoaderService.LoadAsync(sceneName);

                SceneBootstrap sceneBootstrap = Object.FindAnyObjectByType<SceneBootstrap>();

                if (sceneBootstrap == null)
                {
                    throw new NullReferenceException($"Bootstrap for scene: '{sceneName}' not found");
                }

                _currentSceneContainer = new DIContainer(_projectContainer);
                sceneBootstrap.ProcessRegistrations(_currentSceneContainer, sceneArgs);
                _currentSceneContainer.Initialize();

                yield return sceneBootstrap.Initialize();

                coolScreen.ShowPressAnyKey();

                yield return new WaitUntil(() => Input.anyKeyDown);

                _loadingScreen.Hide();

                yield return null;

                sceneBootstrap.Run();
            }
            finally
            {
                _coroutines.StopPerform(hintRoutine);
            }
        }

        private IEnumerator HintCycleRoutine(CoolLoadingScreen screen)
        {
            while (true)
            {
                string randomHint = _loadingHintsService.GetRandomHint();
                screen.SetHint(randomHint);

                yield return new WaitForSeconds(3f + randomHint.Length * 0.05f);
            }
        }
    }
}