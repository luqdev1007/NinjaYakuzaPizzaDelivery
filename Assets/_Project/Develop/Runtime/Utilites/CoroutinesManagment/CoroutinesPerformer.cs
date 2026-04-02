using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment
{
    public class CoroutinesPerformer : MonoBehaviour, ICoroutinesPerformer
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            // kek

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public Coroutine StartPerform(IEnumerator coroutine)
            => StartCoroutine(coroutine);

        public void StopPerform(Coroutine coroutine)
            => StopCoroutine(coroutine);
    }
}