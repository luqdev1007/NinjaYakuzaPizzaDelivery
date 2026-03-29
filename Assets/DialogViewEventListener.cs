using Assets._Project.Develop.Runtime.UI.Dialog;
using UnityEngine;

public class DialogViewEventListener : MonoBehaviour
{
    [SerializeField] private DialogDisplayView _view;

    // Этот метод вызываем из Animation Event в конце анимации появления (Show)
    public void OnShowAnimationEnded()
    {
        if (_view != null)
        {
            _view.OnAppearanceAnimationEnded();
        }
    }

    // Если захочешь ловить конец анимации скрытия (Hide)
    public void OnHideAnimationEnded()
    {
        // Сюда можно добавить вызов, если во вьюхе появится ивент DisappearanceFinished
    }

    // На случай, если забудешь прокинуть ссылку в инспекторе
    private void Reset()
    {
        _view = GetComponentInParent<DialogDisplayView>();
    }
}