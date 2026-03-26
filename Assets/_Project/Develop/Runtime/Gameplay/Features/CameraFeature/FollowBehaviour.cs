using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public class FollowBehaviour : ICameraBehaviour
    {
        private readonly Transform _target;
        private readonly Vector3 _offset;
        private readonly float _smoothSpeed;

        public FollowBehaviour(Transform target, Vector3 offset, float smoothSpeed = 5f)
        {
            _target = target;
            _offset = offset;
            _smoothSpeed = smoothSpeed;
        }

        public Vector3 Update(Vector3 currentPosition, float deltaTime)
        {
            if (_target == null) return currentPosition;

            Vector3 targetPosition = _target.position + _offset;
            return Vector3.Lerp(currentPosition, targetPosition, _smoothSpeed * deltaTime);
        }
    }
}