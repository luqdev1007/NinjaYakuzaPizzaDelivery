using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WallJumpFeature
{
    public class WallJumpSystem : IInitializableSystem, IUpdatableSystem
    {
        private Rigidbody2D _rigidbody;
        private Transform _transform;

        private LayerMask _wallMask;

        private ReactiveVariable<float> _jumpForce;
        private ReactiveVariable<float> _jumpForceMax;
        private ReactiveVariable<bool> _intentJump;

        private ReactiveVariable<Vector2> _moveDirection;

        private int _lastWallDir;
        private float _lastEntrySpeedX;

        public void OnInit(Entity entity)
        {      
            /*
            _rigidbody = entity.Rigidbody;
            _transform = entity.Transform;

            _intentJump = entity.IntentJump;
            _moveDirection = entity.MoveDirection;

            _wallMask = entity.WallMask;

            _jumpForce = entity.JumpForce;
            _jumpForce = entity.JumpForceMax;


            _isWallJumping = entity.IsWallJumping;
            */
        }

        public void OnUpdate(float deltaTime)
        {
            CheckWall();

            if (_intentJump.Value == true)
                PerformWallJump(_lastWallDir);
        }

        private void CheckWall()
        {
            int currentWallDir = GetWallDirection();

            if (currentWallDir != 0)
            {
                _lastWallDir = currentWallDir;

                if (Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f)
                {
                    _lastEntrySpeedX = Mathf.Abs(_rigidbody.linearVelocity.x);
                }
            }
        }

        private int GetWallDirection()
        {
            float checkDist = 0.6f;

            if (Physics2D.Raycast(_transform.position, Vector2.right, checkDist, _wallMask)) 
                return 1;

            if (Physics2D.Raycast(_transform.position, Vector2.left, checkDist, _wallMask)) 
                return -1;

            return 0;
        }

        private void PerformWallJump(int wallDir)
        {
            _moveDirection.Value = new Vector2(-wallDir, _moveDirection.Value.y);

            float bounceForceX = Mathf.Clamp(_lastEntrySpeedX, _jumpForce.Value, _jumpForceMax.Value);
            Vector2 force = new Vector2(-wallDir * bounceForceX, _jumpForce.Value);

            _rigidbody.linearVelocity = force;
            _lastEntrySpeedX = 0;
        }
    }
}