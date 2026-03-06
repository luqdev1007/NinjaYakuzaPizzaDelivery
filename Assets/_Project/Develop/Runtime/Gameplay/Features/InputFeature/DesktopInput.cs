using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const string HorizontalAxisName = "Horizontal";

        public bool IsEnabled { get; set; } = true;

        public Vector2 MoveDirection
        {
            get
            {
                if (IsEnabled == false)
                    return Vector2.zero;
                return new Vector2(Input.GetAxisRaw(HorizontalAxisName), 0);
            }
        }

        public bool IsJumpKeyPressed =>
            IsEnabled && Input.GetKeyDown(KeyCode.Space);

        public bool IsAttackKeyPressed =>
            IsEnabled && Input.GetKeyDown(KeyCode.Mouse0);
    }
}