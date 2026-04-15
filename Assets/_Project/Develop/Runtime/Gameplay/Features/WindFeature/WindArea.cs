using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.WindFeature
{
    public class WindArea : MonoBehaviour
    {
        public Vector2 WindForce; // Направление и сила ветра
        public bool OnlyForGliding; // Влияет ли ветер только когда открыт парашют?
        public float GlideMultiplier = 2.0f; // Усиление ветра при открытом парашюте
    }
}
