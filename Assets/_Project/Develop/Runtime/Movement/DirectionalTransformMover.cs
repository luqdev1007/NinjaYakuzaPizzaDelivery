using UnityEngine;

public class DirectionalTransformMover
{
    private Transform _transform;
    private float _movementSpeed;

    public DirectionalTransformMover(Transform transform, float movementSpeed)
    {
        _transform = transform;
        _movementSpeed = movementSpeed;
    }

    public void Move(Vector3 direction, float deltaTime)
    {
        _transform.Translate(direction * _movementSpeed * deltaTime, Space.World);
    }
}
