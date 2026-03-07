using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public interface IInputService
    {
        bool IsGrappleKeyPressed { get; }
        bool IsGrappleKeyReleased { get; }

        bool IsDashKeyPressed { get; }
        bool IsDashKeyHeld { get; }
        bool IsDashKeyReleased { get; }

        bool IsJumpKeyHeld { get; }   // Space зажат
        bool IsJumpKeyReleased { get; } // Space отпущен

        bool IsEnabled { get; set; }
        Vector2 MoveDirection { get; } 
        bool IsJumpKeyPressed { get; }
        bool IsAttackKeyPressed { get; }
    }
}