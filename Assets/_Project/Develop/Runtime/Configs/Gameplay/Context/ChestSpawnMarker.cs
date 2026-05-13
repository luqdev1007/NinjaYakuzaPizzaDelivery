using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    public class ChestSpawnMarker : MonoBehaviour
    {
        // Здесь можно добавить ссылку на таблицу лута, если нужно
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.7f);
        }
    }
}