using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Core.Buttons;
using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Upgrades
{
    public class UpgradesPanelView : MonoBehaviour, IShowableView
    {
        [field: SerializeField] public HoldButtonHandler CreateMineButton { get; private set; }

        public Tween Hide()
        {
            return null;
        }

        public Tween Show()
        {
            return null;
        }
    }
}