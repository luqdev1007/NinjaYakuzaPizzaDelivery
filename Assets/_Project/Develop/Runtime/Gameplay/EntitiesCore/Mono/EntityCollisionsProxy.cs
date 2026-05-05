using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono
{
    public class EntityCollisionsProxy : MonoBehaviour
    {
        public event Action<Collider2D> OnTriggerEntered;
        public event Action<Collider2D> OnTriggerExited;
        public event Action<Collision2D> OnCollisionStayEvent;

        private void OnTriggerEnter2D(Collider2D other)
        {
            OnTriggerEntered?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnTriggerExited?.Invoke(other);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollisionStayEvent?.Invoke(collision);
        }
    }
}