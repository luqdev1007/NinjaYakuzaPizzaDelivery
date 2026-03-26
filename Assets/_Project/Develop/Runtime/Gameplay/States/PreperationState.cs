using Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero; // Добавлено
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class PreperationState : State, IUpdatableState
    {
        private readonly StartGameTriggerService _startTrigger;
        private readonly CameraService _cameraService;
        private readonly MainHeroFactory _mainHeroFactory; // Добавлено

        private FreePanBehaviour _panBehaviour;

        public PreperationState(
            StartGameTriggerService startTrigger,
            CameraService cameraService,
            MainHeroFactory mainHeroFactory) // Добавлено
        {
            _startTrigger = startTrigger;
            _cameraService = cameraService;
            _mainHeroFactory = mainHeroFactory;
            _panBehaviour = new FreePanBehaviour(25f);
        }

        public override void Enter()
        {
            base.Enter();
            _cameraService.SetBehaviour(_panBehaviour);
        }

        public void Update(float deltaTime)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            _panBehaviour.SetInput(new Vector2(x, y));

            if (Input.GetKeyDown(KeyCode.B))
                _startTrigger.RequestStart();
        }

        public override void Exit()
        {
            base.Exit();

            // Спавним героя ровно в момент выхода из подготовки
            Entity hero = _mainHeroFactory.Create(Vector3.zero);

            // Сразу переключаем камеру на новорожденного героя
            _cameraService.SetBehaviour(new FollowBehaviour(hero.Transform, new Vector3(0, 2, -10)));
        }
    }
}