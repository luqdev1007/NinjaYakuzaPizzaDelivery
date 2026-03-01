using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody;

    private float _damage;

    public void Launch(float damage, float speed)
    {
        _damage = damage;
        _rigidbody.linearVelocity = transform.right * speed;

        Destroy(gameObject, 5);
    }
}