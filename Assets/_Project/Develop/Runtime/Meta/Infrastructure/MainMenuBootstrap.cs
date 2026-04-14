using Assets._Project.Develop.Infrastructure;
using Assets._Project.Develop.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.AudioManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Meta.Infrastructure
{
    public class MainMenuBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private ICoroutinesPerformer _coroutinesPerformer;
        private PlayerDataProvider _playerDataProvider;
        private AudioService _audioService;

        public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
        {
            _container = container;
            MainMenuContextRegistrations.Process(_container);
        }

        public override IEnumerator Initialize()
        {
            _playerDataProvider = _container.Resolve<PlayerDataProvider>();
            _coroutinesPerformer = _container.Resolve<ICoroutinesPerformer>();
            _audioService = _container.Resolve<AudioService>();

            yield break;
        }

        public override void Run()
        {
            // Сбрасываем приглушение мгновенно, чтобы музыка меню сразу звучала сочно
            // _audioService.SetMusicMuted(false);
            _audioService.StartPlaylist("MainMenu");
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