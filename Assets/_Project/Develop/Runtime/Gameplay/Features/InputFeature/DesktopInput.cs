using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const string HorizontalAxisName = "Horizontal";
        private const string VerticalAxisName = "Vertical";

        public bool IsEnabled { get; set; } = true;

        public bool IsAttackKeyPressed => Input.GetKeyDown(KeyCode.Space);

        public Vector2 MoveDirection => new Vector2(Input.GetAxis(HorizontalAxisName), Input.GetAxis(VerticalAxisName));

        public Vector2 RotateDirection { get; set; }

        public bool IsAttackKeyReleased => Input.GetKeyUp(KeyCode.Space);

        public bool IsAttackKeyHold => Input.GetKey(KeyCode.Space);
    }
}