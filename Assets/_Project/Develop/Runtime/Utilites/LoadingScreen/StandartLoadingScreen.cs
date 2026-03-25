using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.LoadingScreen
{
    public class StandartLoadingScreen : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private Animator _animator;

        public bool IsShown => gameObject.activeSelf;

        private void Awake()
        {
            Hide();
            DontDestroyOnLoad(this);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _animator.SetTrigger("Play");
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
