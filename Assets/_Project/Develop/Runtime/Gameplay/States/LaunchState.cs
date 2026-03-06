using Assets._Project.Develop.Runtime.Utilites.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class LaunchState : State, IUpdatableState
    {
        private const float Duration = 3f;

        private float _elapsed;

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
            _elapsed += deltaTime;
            Debug.Log($"Launch countdown: {Mathf.Max(0, Duration - _elapsed):F1}");

            if (_elapsed >= Duration)
                IsFinished = true;
        }


        public override void Exit()
        {
            base.Exit();
            Debug.Log("Launch complete — GO!");
        }
    }
}