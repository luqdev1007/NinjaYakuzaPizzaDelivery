using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Entities.MovementFeature.AirJump;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

public partial class AirJumpSystem : IInitializableSystem, IUpdatableSystem
{
    private ICompositeCondition _canAirJump;

    private ReactiveVariable<bool> _intentJump;

    private ReactiveVariable<float> _airJumpForce;
    private ReactiveVariable<float> _airJumpForceMax;
    private ReactiveVariable<float> _airJumpChargeTime;
    private ReactiveVariable<int> _airJumpsCount;
    private ReactiveEvent _airJumpEvent;

    private Rigidbody2D _rigidbody;

    private float _chargeTimer;
    private float _jumpBufferTimer;

    private bool _isCharging;
    private bool _wasJumpIntendedLastFrame;

    private const float JumpBufferTime = 0.15f;

    public void OnInit(Entity entity)
    {
        _intentJump = entity.IntentJump;

        _canAirJump = entity.CanAirJump;

        _airJumpChargeTime = entity.AirJumpChargeTime;

        _airJumpsCount = entity.AirJumpsCount;

        _airJumpEvent = entity.AirJumpEvent;

        _airJumpForce = entity.AirJumpForceMin;
        _airJumpForceMax = entity.AirJumpForceMax;

        _rigidbody = entity.Rigidbody;
    }

    public void OnUpdate(float deltaTime)
    {
        bool currentIntent = _intentJump.Value;
        bool isJumpPressedDown = currentIntent && !_wasJumpIntendedLastFrame;
        bool isJumpReleased = !currentIntent && _wasJumpIntendedLastFrame;

        _wasJumpIntendedLastFrame = currentIntent;

        if (isJumpPressedDown)
            _jumpBufferTimer = JumpBufferTime;
        else
            _jumpBufferTimer -= deltaTime;

        if (_jumpBufferTimer > 0f && _canAirJump.Evaluate() && !_isCharging)
        {
            _isCharging = true;
            _chargeTimer = 0f;
            _jumpBufferTimer = 0f;
        }

        if (_isCharging)
        {
            if (!_canAirJump.Evaluate())
            {
                _isCharging = false;
                return;
            }

            if (currentIntent)
            {
                _chargeTimer += deltaTime;

                if (_chargeTimer >= _airJumpChargeTime.Value)
                {
                    ExecuteAirJump();
                }
            }
            else if (isJumpReleased)
            {
                ExecuteAirJump();
            }
        }
    }

    private void ExecuteAirJump()
    {
        float chargeRatio = _airJumpChargeTime.Value > 0f ? _chargeTimer / _airJumpChargeTime.Value : 1f;
        float verticalForce = Mathf.Lerp(_airJumpForce.Value, _airJumpForceMax.Value, chargeRatio);

        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
        _rigidbody.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);

        _airJumpEvent?.Invoke();
        _airJumpsCount.Value--;
        Debug.Log($"air jump event! Left jumps: {_airJumpsCount.Value}");

        _isCharging = false;
    }
}
