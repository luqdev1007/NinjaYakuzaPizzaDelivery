using UnityEngine;
using Cainos.Common;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;

namespace Cainos.PixelArtPlatformer_VillageProps
{
    public class BoundingPlatform : MonoBehaviour
    {
        public float pushDistance = 2.0f;
        public float waitTime = 1.0f;
        public float pushForce = 20.0f;

        private Rigidbody2D rb;
        private Vector2 startPos;

        private float targetOffset;
        private float waitTimer;
        private bool movingUp = true;

        private SecondOrderDynamics secondOrderDynamics = new SecondOrderDynamics(4.0f, 0.5f, -0.3f);

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            startPos = rb.position;
            secondOrderDynamics.Reset(0.0f);
        }

        private void FixedUpdate()
        {
            waitTimer += Time.fixedDeltaTime;

            if (waitTimer > waitTime)
            {
                if (movingUp)
                {
                    targetOffset = pushDistance;
                    if (Mathf.Abs(targetOffset - secondOrderDynamics.Update(targetOffset, Time.fixedDeltaTime)) < 0.1f)
                    {
                        waitTimer = 0.0f;
                        movingUp = false;
                    }
                }
                else
                {
                    targetOffset = 0.0f;
                    if (Mathf.Abs(secondOrderDynamics.Update(targetOffset, Time.fixedDeltaTime)) < 0.1f)
                    {
                        waitTimer = 0.0f;
                        movingUp = true;
                    }
                }
            }

            float currentOffset = secondOrderDynamics.Update(targetOffset, Time.fixedDeltaTime);
            Vector2 nextPos = startPos + (Vector2)transform.up * currentOffset;
            rb.MovePosition(nextPos);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            MonoEntity entity = collision.gameObject.GetComponentInParent<MonoEntity>();

            if (entity != null)
            {
                Rigidbody2D otherRb = entity.GetComponent<Rigidbody2D>();
                if (otherRb != null)
                {
                    Vector2 force = transform.up * pushForce;
                    otherRb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }
    }
}