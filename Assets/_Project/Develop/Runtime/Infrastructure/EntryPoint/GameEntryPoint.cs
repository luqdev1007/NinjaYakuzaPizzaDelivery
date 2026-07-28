using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Infrastructure.EntryPoint
{
    public class GameEntryPoint : MonoBehaviour
    {
        private void Awake()
        {
            SetupAppSettings();

            DIContainer projectContainer = new DIContainer();
            ProjectContextRegistrations.Process(projectContainer);
            projectContainer.Initialize();

            ICoroutinesPerformer coroutinePerformer = projectContainer.Resolve<ICoroutinesPerformer>();
            coroutinePerformer.StartPerform(Initialize(projectContainer));
        }

        private void SetupAppSettings()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private IEnumerator Initialize(DIContainer container)
        {
            SceneSwitcherService sceneSwitcherService = container.Resolve<SceneSwitcherService>();
            PlayerDataProvider playerDataProvider = container.Resolve<PlayerDataProvider>();

            yield return container.Resolve<ConfigsProviderService>().LoadAsync();

            bool isPlayerDataSaveExists = false;

            yield return playerDataProvider.ExistsAsync(result => isPlayerDataSaveExists = result);

            if (isPlayerDataSaveExists)
                yield return playerDataProvider.LoadAsync();
            else
                playerDataProvider.Reset();

            // Читаем флаг из in-memory модели, а не перепроверяем файл: Reset() уже
            // стартовал запись сейва корутиной, и после неё "сейва нет" превратится
            // в "сейв есть". IntroSeen=true проставляет IntroBootstrap в конце интро.
            if (playerDataProvider.IntroSeen == false)
            {
                yield return sceneSwitcherService.ProcessingSwitchTo(Scenes.Intro);
            }
            else
            {
                yield return sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu);
            }
        }
    }
}
