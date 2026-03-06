using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

public class JumpSystem : IInitializableSystem, IUpdatableSystem
{
    private readonly IInputService _inputService;

    private ReactiveVariable<bool> _isGrounded;
    private ReactiveVariable<int> _jumpsAvailable;
    private ReactiveVariable<int> _maxJumps;
    private ReactiveVariable<float> _jumpForce;
    private ReactiveVariable<float> _jumpForceMax;
    private ReactiveVariable<float> _jumpChargeTime;
    private Rigidbody2D _rigidbody;

    private float _chargeTimer;
    private bool _isCharging;

    private ReactiveVariable<float> _minFallVelocity;

    public JumpSystem(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void OnInit(Entity entity)
    {
        _isGrounded = entity.IsGrounded;
        _jumpsAvailable = entity.JumpsAvailable;
        _maxJumps = entity.MaxJumps;
        _jumpForce = entity.JumpForce;
        _jumpForceMax = entity.JumpForceMax;
        _jumpChargeTime = entity.JumpChargeTime;
        _rigidbody = entity.Rigidbody;

        _minFallVelocity = entity.MinFallVelocityForAction;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_isGrounded.Value)
            _jumpsAvailable.Value = _maxJumps.Value;

        if (_inputService.IsJumpKeyPressed
            && _jumpsAvailable.Value > 0
            && _rigidbody.linearVelocity.y >= _minFallVelocity.Value)
        {
            _isCharging = true;
            _chargeTimer = 0f;
        }

        if (_isCharging && _inputService.IsJumpKeyHeld)
        {
            _chargeTimer = Mathf.Min(
                _chargeTimer + deltaTime,
                _jumpChargeTime.Value);
        }

        if (_isCharging && _inputService.IsJumpKeyReleased)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        float chargeRatio = _jumpChargeTime.Value > 0
            ? _chargeTimer / _jumpChargeTime.Value
            : 1f;

        float force = Mathf.Lerp(
            _jumpForce.Value,
            _jumpForceMax.Value,
            chargeRatio);

        _rigidbody.linearVelocity = new Vector2(
            _rigidbody.linearVelocity.x, 0);

        _rigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);

        _jumpsAvailable.Value--;
        _isCharging = false;
        _chargeTimer = 0f;
    }
}