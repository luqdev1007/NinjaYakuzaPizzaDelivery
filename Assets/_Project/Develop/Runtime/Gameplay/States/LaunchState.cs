using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class LaunchState : State, IUpdatableState
    {
        private float _elapsed;

        public LaunchState(float duration = 0)
        {
            Duration = duration;
        }

        public float Duration { get; private set; }

        public bool IsFinished { get; private set; }

        public override void Enter()
        {
            base.Enter();
            _elapsed = 0f;
            IsFinished = false;
            Debug.Log("Заказ принят! Ниндзя отправляется в путь...");
        }

        public void Update(float deltaTime)
        {
            Debug.Log("Press any key to start timer");

            if (Input.GetKeyDown(KeyCode.Escape) == false && (Input.GetKeyDown(KeyCode.R) == false && Input.anyKeyDown))
            {
                IsFinished = true;
            }
        }


        public override void Exit()
        {
            base.Exit();
            Debug.Log("Launch complete — GO!");
        }
    }
}