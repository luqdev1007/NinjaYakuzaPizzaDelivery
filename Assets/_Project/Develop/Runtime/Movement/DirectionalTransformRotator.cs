using UnityEngine;

public class DirectionalTransformRotator
{
    private Transform _transform;

    public DirectionalTransformRotator(Transform transform)
    {
        _transform = transform;
    }

    public void Rotate(Vector3 direction, float deltaTime)
    {
        Vector3 desiredRotation = (direction - _transform.position).normalized;
        float angle = Mathf.Atan2(desiredRotation.y, desiredRotation.x) * Mathf.Rad2Deg;
        _transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
