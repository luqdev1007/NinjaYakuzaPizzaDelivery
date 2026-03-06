using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

public class JumpSystem : IInitializableSystem, IUpdatableSystem
{
    private ReactiveVariable<bool> _isGrounded;
    private ReactiveVariable<int> _jumpsAvailable;
    private ReactiveVariable<int> _maxJumps;
    private ReactiveVariable<float> _jumpForce;
    private Rigidbody2D _rigidbody;

    public void OnInit(Entity entity)
    {
        _isGrounded = entity.IsGrounded;
        _jumpsAvailable = entity.JumpsAvailable;
        _maxJumps = entity.MaxJumps;
        _jumpForce = entity.JumpForce;
        _rigidbody = entity.Rigidbody;
        // JumpRequest убрали
    }

    public void OnUpdate(float deltaTime)
    {
        if (_isGrounded.Value)
            _jumpsAvailable.Value = _maxJumps.Value;
    }

    public void TryJump()
    {
        Debug.Log($"TryJump called. JumpsAvailable: {_jumpsAvailable.Value}, IsGrounded: {_isGrounded.Value}");

        if (_jumpsAvailable.Value <= 0)
        {
            Debug.Log("Jump blocked - no jumps available");
            return;
        }

        _rigidbody.linearVelocity = new Vector2(
            _rigidbody.linearVelocity.x, 0);

        _rigidbody.AddForce(Vector2.up * _jumpForce.Value, ForceMode2D.Impulse);
        _jumpsAvailable.Value--;
    }
}