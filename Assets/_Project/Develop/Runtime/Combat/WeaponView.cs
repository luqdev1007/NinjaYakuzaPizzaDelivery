using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private Vector3 _baseOffset;

    public Transform ShootPoint => _shootPoint;

    private void OnEnable()
    {
        transform.localPosition = _baseOffset;
    }
}
