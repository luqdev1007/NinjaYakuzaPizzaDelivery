using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.CameraFeature
{
    public interface ICameraBehaviour
    {
        Vector3 Update(Vector3 currentPosition, float deltaTime);
    }
}