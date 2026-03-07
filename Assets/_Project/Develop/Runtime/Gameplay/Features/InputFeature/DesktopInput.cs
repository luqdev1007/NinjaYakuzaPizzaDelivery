using UnityEngine;
namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const string HorizontalAxisName = "Horizontal";
        public bool IsGrappleKeyPressed => Input.GetMouseButtonDown(1);
        public bool IsGrappleKeyReleased => Input.GetMouseButtonUp(1);
        public bool IsDashKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.LeftShift);
        public bool IsDashKeyHeld => IsEnabled && Input.GetKey(KeyCode.LeftShift);
        public bool IsDashKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.LeftShift);
        public bool IsEnabled { get; set; } = true;
        public bool IsJumpKeyHeld => IsEnabled && Input.GetKey(KeyCode.Space);
        public bool IsJumpKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.Space);
        public bool IsJumpKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.Space);
        public bool IsAttackKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.Mouse0);
        public bool IsAttackKeyHeld => IsEnabled && Input.GetKey(KeyCode.Mouse0);
        public bool IsAttackKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.Mouse0);
        public bool IsSlideKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.LeftControl);
        public bool IsSlideKeyHeld => IsEnabled && Input.GetKey(KeyCode.LeftControl);
        public bool IsSlideKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.LeftControl);
        public Vector2 MoveDirection
        {
            get
            {
                if (IsEnabled == false)
                    return Vector2.zero;
                return new Vector2(Input.GetAxisRaw(HorizontalAxisName), 0);
            }
        }
    }
}