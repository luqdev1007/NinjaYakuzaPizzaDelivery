using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Infrastructure.EntryPoint
{
    public class GameEntryPoint : MonoBehaviour
    {
        private readonly List<string> _loadingHints = new List<string>
        {
            "Ниндзя не едят ананасы в пицце.",
            "Если пицца остыла, она становится метательным диском.",
            "Доставка за 30 секунд или харакири.",
            "Секретный ингредиент — это скорость.",
            "Крути тесто так, будто это твой враг."
        };

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
            var loadingScreen = (CoolLoadingScreen)container.Resolve<ILoadingScreen>();
            SceneSwitcherService sceneSwitcherService = container.Resolve<SceneSwitcherService>();
            PlayerDataProvider playerDataProvider = container.Resolve<PlayerDataProvider>();

            loadingScreen.Show();

            Coroutine hintRoutine = StartCoroutine(HintCycleRoutine(loadingScreen));

            yield return container.Resolve<ConfigsProviderService>().LoadAsync();

            bool isPlayerDataSaveExists = false;
            yield return playerDataProvider.ExistsAsync(result => isPlayerDataSaveExists = result);

            if (isPlayerDataSaveExists)
                yield return playerDataProvider.LoadAsync();
            else
                playerDataProvider.Reset();

            yield return new WaitForSeconds(1.5f); 

            StopCoroutine(hintRoutine);
            loadingScreen.SetHint("Загрузка завершена!");
            loadingScreen.ShowPressAnyKey();

            yield return new WaitUntil(() => Input.anyKeyDown);

            loadingScreen.Hide();
            yield return sceneSwitcherService.ProcessingSwitchTo(Scenes.MainMenu);
        }

        private IEnumerator HintCycleRoutine(CoolLoadingScreen screen)
        {
            while (true)
            {
                string randomHint = _loadingHints[Random.Range(0, _loadingHints.Count)];
                screen.SetHint(randomHint);
                yield return new WaitForSeconds(3f);
            }
        }
    }
}