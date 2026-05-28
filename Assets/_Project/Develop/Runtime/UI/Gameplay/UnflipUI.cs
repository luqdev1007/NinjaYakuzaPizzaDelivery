using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    [ExecuteAlways]
    public class UnflipUI : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (transform.parent == null)
                return;

            float parentYRotation = transform.parent.eulerAngles.y;

            bool isParentFlipped = Mathf.Abs(Mathf.DeltaAngle(parentYRotation, 180f)) < 1f;

            Vector3 localRotation = transform.localRotation.eulerAngles;
            float targetLocalY = isParentFlipped ? 180f : 0f;

            if (!Mathf.Approximately(localRotation.y, targetLocalY))
            {
                transform.localRotation = Quaternion.Euler(localRotation.x, targetLocalY, localRotation.z);
            }
        }
    }
}