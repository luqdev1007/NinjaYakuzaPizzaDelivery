using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Vehicles
{
    public class ShipPlace : MonoBehaviour
    {
        [field: SerializeField] public ShipPlaceType PlaceType { get; private set; }
    }
}
