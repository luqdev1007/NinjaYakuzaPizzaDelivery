using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;
using Assets._Project.Develop.Runtime.Utilites.Conditions;

public class FlipDirectionSystem : IInitializableSystem, IUpdatableSystem
{
    private ReactiveVariable<Vector2> _direction;
    private Transform _transform;
    private ICompositeCondition _canFlipDirection;

    public void OnInit(Entity entity)
    {
        _direction = entity.MoveDirection;
        _transform = entity.Transform;
        _canFlipDirection = entity.CanFlipDirection;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_canFlipDirection.Evaluate() == false)
            return;

        if (_direction.Value.x == 0)
            return;

        Vector3 scale = _transform.localScale;
        scale.x = _direction.Value.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        _transform.localScale = scale;
    }
}
