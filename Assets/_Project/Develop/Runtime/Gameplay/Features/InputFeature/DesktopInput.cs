using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class DesktopInput : IInputService
    {
        private const string HorizontalAxis = "Horizontal";
        private const string VerticalAxis = "Vertical";
        private const string MouseScrollWheel = "Mouse ScrollWheel";

        public bool IsEnabled { get; set; } = true;

        // Movement & Camera
        public Vector2 MoveDirection => IsEnabled
            ? new Vector2(Input.GetAxisRaw(HorizontalAxis), 0)
            : Vector2.zero;

        public Vector2 CameraMoveDirection => IsEnabled
            ? new Vector2(Input.GetAxisRaw(HorizontalAxis), Input.GetAxisRaw(VerticalAxis))
            : Vector2.zero;

        public float MouseScrollDelta => IsEnabled
            ? Input.GetAxisRaw(MouseScrollWheel)
            : 0f;

        // Combat & Actions
        public bool IsAttackKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.Mouse0);
        public bool IsAttackKeyHeld => IsEnabled && Input.GetKey(KeyCode.Mouse0);
        public bool IsAttackKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.Mouse0);

        public bool IsThrowKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.Q);

        // Grapple (Right Mouse Button)
        public bool IsGrappleKeyPressed => IsEnabled && Input.GetMouseButtonDown(1);
        public bool IsGrappleKeyHeld => IsEnabled && Input.GetMouseButton(1);
        public bool IsGrappleKeyReleased => IsEnabled && Input.GetMouseButtonUp(1);

        // Jump & Mobility
        public bool IsJumpKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.Space);
        public bool IsJumpKeyHeld => IsEnabled && Input.GetKey(KeyCode.Space);
        public bool IsJumpKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.Space);

        public bool IsDashKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.LeftShift);
        public bool IsDashKeyHeld => IsEnabled && Input.GetKey(KeyCode.LeftShift);
        public bool IsDashKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.LeftShift);

        public bool IsSlideKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.LeftControl);
        public bool IsSlideKeyHeld => IsEnabled && Input.GetKey(KeyCode.LeftControl);
        public bool IsSlideKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.LeftControl);

        // Interaction
        public bool IsInteractKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.E);
        public bool IsInteractKeyHeld => IsEnabled && Input.GetKey(KeyCode.E);
        public bool IsInteractKeyReleased => IsEnabled && Input.GetKeyUp(KeyCode.E);

        // System
        public bool IsStartLevelKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.T);
        public bool IsRestartKeyPressed => IsEnabled && Input.GetKeyDown(KeyCode.R);

        // TARGET
        public bool IsTargetLockKeyHeld => Input.GetKey(KeyCode.Tab);
    }
}