using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

public class DojoView : MonoBehaviour, IView
{
    [field: SerializeField] public Button ExitDojoButton { get; private set; }
}
