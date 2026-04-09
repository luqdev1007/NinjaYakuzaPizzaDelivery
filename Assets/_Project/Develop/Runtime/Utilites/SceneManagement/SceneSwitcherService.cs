using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Utilites.SceneManagement
{
    public class SceneSwitcherService
    {
        private readonly SceneLoaderService _sceneLoaderService;
        private readonly ILoadingScreen _loadingScreen;
        private readonly DIContainer _projectContainer;
        private readonly ICoroutinesPerformer _coroutines; 

        private DIContainer _currentSceneContainer;

        private readonly List<string> _loadingHints = new List<string>
        {
            "A $3.50 tip is a great start for a novice ninja",
            "The cake is a lie, but the pizza is real",
            "Beware of spooky red ghosts",
            "Short or long click near any wall to hang on it",
            "Hold the jump key to control your jump height",
            "You have double jumps! They refresh once you touch the ground",
            "Slopes are perfect for sliding",
            "Dashing makes you invulnerable for a few seconds",
            "I’m not superstitious, but I am a little stitious about delivery times",
            "I want customers to be afraid of how much they love my pizza",
            "You miss 100% of the pizzas you don't bake. — Michael Scott",
            "A real ninja never quits. Unless his shift is over",
            "Is a pepperoni slice a deadly shuriken? Only if you believe in yourself",
            "Business is like a katana. If you hold it by the wrong end, you’re gonna have a bad day"
        };

        public SceneSwitcherService(
            SceneLoaderService sceneLoaderService,
            ILoadingScreen loadingScreen,
            DIContainer projectContainer,
            ICoroutinesPerformer coroutines)
        {
            _sceneLoaderService = sceneLoaderService;
            _loadingScreen = loadingScreen;
            _projectContainer = projectContainer;
            _coroutines = coroutines;
        }

        public IEnumerator ProcessingSwitchTo(string sceneName, IInputSceneArgs sceneArgs = null)
        {
            CoolLoadingScreen coolScreen = _loadingScreen as CoolLoadingScreen;
            Coroutine hintRoutine = _coroutines.StartPerform(HintCycleRoutine(coolScreen));

            _loadingScreen.Show();

            _currentSceneContainer?.Dispose();

            yield return _sceneLoaderService.LoadAsync(Scenes.Empty);
            yield return _sceneLoaderService.LoadAsync(sceneName);

            SceneBootstrap sceneBootstrap = Object.FindFirstObjectByType<SceneBootstrap>();

            if (sceneBootstrap == null)
                throw new NullReferenceException($"Bootstrap for scene: '{sceneName}' not found");

            _currentSceneContainer = new DIContainer(_projectContainer);
            sceneBootstrap.ProcessRegistrations(_currentSceneContainer, sceneArgs);
            _currentSceneContainer.Initialize();

            // long loadings tests
            /*
            float timer = 5;

            while (timer > 0)
            {
                timer -= Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.Escape))
                    yield break;

                yield return null;
            }

            timer = 0;
            */
            // long loadings tests

            yield return sceneBootstrap.Initialize();

            /*
            if (hintRoutine != null) 
                _coroutines.StopPerform(hintRoutine);
            */

            // coolScreen.SetHint("");

            coolScreen.ShowPressAnyKey();

            yield return new WaitUntil(() => Input.anyKeyDown);

            _loadingScreen.Hide();

            sceneBootstrap.Run();
        }

        private IEnumerator HintCycleRoutine(CoolLoadingScreen screen)
        {
            while (true)
            {
                string randomHint = _loadingHints[Random.Range(0, _loadingHints.Count)];
                screen.SetHint(randomHint);

                yield return new WaitForSeconds(3f + randomHint.Length * 0.05f);
            }
        }
    }
}