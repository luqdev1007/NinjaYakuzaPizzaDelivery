using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay
{
    public class CameraFollowService
    {
        private readonly Camera _camera;
        private Transform _target;

        [SerializeField] private float _smoothSpeed = 5f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -10f);

        public CameraFollowService(Camera camera)
        {
            _camera = camera;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _camera.transform.SetParent(null);
        }

        public void Update(float deltaTime)
        {
            if (_target == null)
                return;

            Vector3 targetPosition = _target.position + _offset;
            _camera.transform.position = Vector3.Lerp(
                _camera.transform.position,
                targetPosition,
                _smoothSpeed * deltaTime);
        }
    }
}