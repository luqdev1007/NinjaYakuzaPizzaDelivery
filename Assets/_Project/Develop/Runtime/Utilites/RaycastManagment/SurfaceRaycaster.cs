namespace Assets._Project.Develop.Runtime.Utilites.RaycastManagment
{
    using UnityEngine;

    public class SurfaceRaycaster
    {
        public bool TryGetHitInfo(Camera from, LayerMask hitMask, out RaycastHit hit)
        {
            Ray ray = from.ScreenPointToRay(Input.mousePosition);

            return Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask);
        }
    }
}
