using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Background
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] private float _parallaxSpeed = 0.1f;
        [SerializeField] private bool _infiniteScroll = true;

        private Transform _cameraTransform;
        private Vector3 _lastCameraPosition;
        private float _textureUnitSizeX;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
            _lastCameraPosition = _cameraTransform.position;

            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
            _textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        }

        private void LateUpdate()
        {
            Vector3 delta = _cameraTransform.position - _lastCameraPosition;
            transform.position += new Vector3(delta.x * _parallaxSpeed, 0f);
            _lastCameraPosition = _cameraTransform.position;

            if (_infiniteScroll)
            {
                float distanceFromCamera = _cameraTransform.position.x - transform.position.x;
                if (Mathf.Abs(distanceFromCamera) >= _textureUnitSizeX)
                {
                    float offset = distanceFromCamera % _textureUnitSizeX;
                    transform.position = new Vector3(
                        _cameraTransform.position.x + offset,
                        transform.position.y,
                        transform.position.z);
                }
            }
        }
    }
}