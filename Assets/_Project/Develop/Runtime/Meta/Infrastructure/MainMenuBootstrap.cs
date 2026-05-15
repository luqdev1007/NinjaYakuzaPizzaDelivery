using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AudioManagment;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private ICoroutinesPerformer _coroutinesPerformer;
        private PlayerDataProvider _playerDataProvider;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
            MainMenuContextRegistrations.Process(_container);
        }

        public override IEnumerator Initialize()
        {
            _playerDataProvider = _container.Resolve<PlayerDataProvider>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            yield break;
        }

        public override void Run()
        {
            IAudioService audioService = _container.Resolve<IAudioService>();
            audioService.PlayPlaylist("MainMenu_Playlist");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                _coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());
            }
        }
    }
}