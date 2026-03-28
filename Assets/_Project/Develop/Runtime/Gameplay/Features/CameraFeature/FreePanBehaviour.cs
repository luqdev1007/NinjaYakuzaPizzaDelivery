using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class FreePanBehaviour : ICameraBehaviour
    {
        private readonly float _panSpeed;
        private readonly float _boostMultiplier;
        private Vector2 _currentInput;

        public FreePanBehaviour(float panSpeed = 20f, float boostMultiplier = 2f)
        {
            _panSpeed = panSpeed;
            _boostMultiplier = boostMultiplier;
        }

        public void SetInput(Vector2 input)
        {
            // Убираем normalized здесь, если хочешь учитывать силу отклонения стика, 
            // но для клавиатуры normalized обычно полезен, чтобы по диагонали не летать быстрее.
            _currentInput = input;
        }

        public Vector3 Update(Vector3 currentPosition, float deltaTime)
        {
            // Проверяем нажатие Shift (Left или Right)
            float currentMultiplier = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                ? _boostMultiplier
                : 1f;

            // Считаем финальный вектор движения
            Vector3 movement = (Vector3)_currentInput.normalized * (_panSpeed * currentMultiplier) * deltaTime;

            Vector3 nextPos = currentPosition + movement;

            // Обнуляем инпут после использования
            _currentInput = Vector2.zero;

            return nextPos;
        }
    }
}