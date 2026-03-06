using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public interface IInputService
    {
        bool IsJumpKeyHeld { get; }   // Space зажат
        bool IsJumpKeyReleased { get; } // Space отпущен

        bool IsEnabled { get; set; }
        Vector2 MoveDirection { get; } 
        bool IsJumpKeyPressed { get; }
        bool IsAttackKeyPressed { get; }
    }
}