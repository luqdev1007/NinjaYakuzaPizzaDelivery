using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilites.SceneManagement;
using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class PreperationState : State, IUpdatableState
    {
        private readonly GameplayInputArgs _inputArgs;
        private readonly StartGameTriggerService _startTrigger;

        public PreperationState(
            GameplayInputArgs inputArgs,
            StartGameTriggerService startTrigger)
        {
            _inputArgs = inputArgs;
            _startTrigger = startTrigger;
        }

        public override void Enter()
        {
            base.Enter();
            _startTrigger.Reset();
            Debug.Log("Prep state — ожидание нажатия Начать");
        }

        public void Update(float deltaTime)
        {
            // пока тест — пробел запускает старт
            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
                _startTrigger.RequestStart();
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("Prep exit");
        }
    }
}