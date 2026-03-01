using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public interface IInputService
    {
        bool IsEnabled { get; set; }

        Vector2 MoveDirection { get; }
        Vector2 RotateDirection { get; }

        bool IsAttackKeyPressed { get; }
        bool IsAttackKeyReleased { get; }
        bool IsAttackKeyHold { get; }
    }
}
