using DG.Tweening;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Buffs
{
    public class BuffsView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _rootGroup;
        [SerializeField] private Transform _body;
        [SerializeField] private BuffView _buffViewPrefab;

        public BuffView CreateBuffView()
        {
            return Instantiate(_buffViewPrefab, _body);
        }

        public void Hide()
        {
            if (_rootGroup != null)
            {
                _rootGroup.DOFade(0f, 0.5f).SetUpdate(true);
            }
        }
    }
}