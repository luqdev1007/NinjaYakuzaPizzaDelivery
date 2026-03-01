using Assets._Project.Develop.Runtime.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayScreenView : MonoBehaviour, IView
    {
        [field: SerializeField] public Button BackToMenuButton { get; private set; }

        public void Init()
        {
            Debug.Log("Gameplay screen view Inited");
        }
    }
}