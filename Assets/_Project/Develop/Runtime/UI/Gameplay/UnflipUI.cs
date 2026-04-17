using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    [ExecuteAlways]
    public class UnflipUI : MonoBehaviour
    {
        private void LateUpdate()
        {
            Vector3 localScale = transform.localScale;

            // Если родитель отзеркален (scale.x < 0), мы инвертируем свой scale, 
            // чтобы в мировых координатах всегда быть "прямыми"
            if (transform.parent != null && transform.parent.lossyScale.x < 0)
            {
                if (localScale.x > 0)
                {
                    localScale.x *= -1;
                    transform.localScale = localScale;
                }
            }
            else if (localScale.x < 0)
            {
                localScale.x *= -1;
                transform.localScale = localScale;
            }
        }
    }
}