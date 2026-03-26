using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class FreePanBehaviour : ICameraBehaviour
    {
        private readonly float _panSpeed;
        private Vector2 _currentInput;

        public FreePanBehaviour(float panSpeed = 20f)
        {
            _panSpeed = panSpeed;
        }

        public void SetInput(Vector2 input)
        {
            _currentInput = input.normalized;
        }

        public Vector3 Update(Vector3 currentPosition, float deltaTime)
        {
            Vector3 nextPos = currentPosition + (Vector3)_currentInput * _panSpeed * deltaTime;
            _currentInput = Vector2.zero;
            return nextPos;
        }
    }
}