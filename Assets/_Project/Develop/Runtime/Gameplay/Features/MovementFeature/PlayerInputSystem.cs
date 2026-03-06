using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
{
    private readonly IInputService _inputService;
    private ReactiveVariable<Vector2> _moveDirection;

    public PlayerInputSystem(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void OnInit(Entity entity)
    {
        _moveDirection = entity.MoveDirection;
    }

    public void OnUpdate(float deltaTime)
    {
        _moveDirection.Value = _inputService.MoveDirection;
    }
}