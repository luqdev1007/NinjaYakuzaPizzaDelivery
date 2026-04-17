using UnityEngine;

public class WindmillPhysicsHandler : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private float _maxMotorForce = 10000f;

    private HingeJoint2D _hinge;

    private void Awake()
    {
        _hinge = GetComponent<HingeJoint2D>();

        // Включаем мотор программно, чтобы не забыть в инспекторе
        _hinge.useMotor = true;
    }

    private void FixedUpdate()
    {
        // Обновляем параметры мотора (полезно, если хочешь менять скорость в рантайме)
        JointMotor2D motor = _hinge.motor;
        motor.motorSpeed = _rotationSpeed;
        motor.maxMotorTorque = _maxMotorForce;
        _hinge.motor = motor;
    }
}
