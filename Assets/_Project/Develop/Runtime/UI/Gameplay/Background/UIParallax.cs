using UnityEngine;

public class UIParallax : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    [Tooltip("0.1 для заднего, -0.05 для переднего")]
    [SerializeField] private float _parallaxEffect; 

    private Vector3 _startPosition;
    private Vector3 _startCameraPos;

    void Start()
    {
        _startPosition = transform.localPosition;
        _startCameraPos = _cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 diff = _cameraTransform.position - _startCameraPos;
        transform.localPosition = _startPosition + (diff * _parallaxEffect);
    }
}