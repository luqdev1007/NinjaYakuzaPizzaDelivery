using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterConfig _config;
    [SerializeField] private Transform _weaponPoint;

    [SerializeField] private WeaponConfig _pistolConfig;
    [SerializeField] private WeaponConfig _shotgunConfig;

    private PCPlayerInputHandler _inputHandler;
    private DirectionalTransformMover _mover;
    private DirectionalTransformRotator _rotator;

    private Weapon _currentWeapon;

    private void Awake()
    {
        _inputHandler = new PCPlayerInputHandler();
        _mover = new DirectionalTransformMover(transform, _config.MovementSpeed);
        _rotator = new DirectionalTransformRotator(transform);
    }

    private void Update()
    {
        Vector3 movementDirection = _inputHandler.GetMovementDirection();
        Vector3 rotationDirection = _inputHandler.GetRotationDirection();

        _mover.Move(movementDirection, Time.deltaTime);
        _rotator.Rotate(rotationDirection, Time.deltaTime);

        if (_inputHandler.IsShootKeyPressing())
        {
            if (_currentWeapon != null)
                _currentWeapon.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_currentWeapon != null)
                _currentWeapon.Dispose();

            _currentWeapon = new Pistol(_pistolConfig, _weaponPoint, this);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_currentWeapon != null)
                _currentWeapon.Dispose();

            _currentWeapon = new Shotgun(_shotgunConfig, _weaponPoint, this);
        }
    }
}
