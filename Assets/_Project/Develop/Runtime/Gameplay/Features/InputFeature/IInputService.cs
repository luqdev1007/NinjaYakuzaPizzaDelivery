using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public interface IInputService
    {
        bool IsInteractKeyPressed { get; } 
        bool IsInteractKeyHeld { get; } 
        bool IsInteractKeyReleased { get; } 


        bool IsStartLevelKeyPressed { get; } 

        Vector2 CameraMoveDirection { get; }
        bool IsThrowKeyPressed { get; } 
        float MouseScrollDelta { get; } 

        bool IsAttackKeyHeld { get; }
        bool IsAttackKeyReleased { get; }
        bool IsSlideKeyPressed { get; }
        bool IsSlideKeyHeld { get; }
        bool IsSlideKeyReleased { get; }

        bool IsGrappleKeyPressed { get; }
        bool IsGrappleKeyHeld { get; } 
        bool IsGrappleKeyReleased { get; }        

        bool IsDashKeyPressed { get; }
        bool IsDashKeyHeld { get; }
        bool IsDashKeyReleased { get; }

        bool IsJumpKeyHeld { get; }
        bool IsJumpKeyReleased { get; }

        bool IsEnabled { get; set; }
        Vector2 MoveDirection { get; }
        bool IsJumpKeyPressed { get; }
        bool IsAttackKeyPressed { get; }
        bool IsRestartKeyPressed { get; }

        bool IsTargetLockKeyHeld { get; }
    }
}