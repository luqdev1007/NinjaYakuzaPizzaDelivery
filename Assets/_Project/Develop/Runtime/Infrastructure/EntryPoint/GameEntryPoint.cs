using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
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

            // Загружаем конфиги
            yield return container.Resolve<ConfigsProviderService>().LoadAsync();

            // Работаем с данными игрока
            bool isPlayerDataSaveExists = false;
            yield return playerDataProvider.ExistsAsync(result => isPlayerDataSaveExists = result);

            if (isPlayerDataSaveExists)
                yield return playerDataProvider.LoadAsync();
            else
                playerDataProvider.Reset();

            // Переходим в меню. Сервис сам покажет экран, подсказки и подождет AnyKey.
            yield return sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu);
        }
    }
}