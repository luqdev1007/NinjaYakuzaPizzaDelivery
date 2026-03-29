using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.SlopeFeature
{
    public class EntityCollisionProxy : MonoBehaviour
    {
        public event Action<Collision2D> OnCollisionStayEvent;

        private void OnCollisionStay2D(Collision2D collision)
        {
            OnCollisionStayEvent?.Invoke(collision);
        }
    }
}