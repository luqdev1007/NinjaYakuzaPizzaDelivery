using Assets._Project.Develop.Runtime.UI.Dialog;
using UnityEngine;

public class DialogViewEventListener : MonoBehaviour
{
    [SerializeField] private DialogDisplayView _view;

    public void OnShowAnimationEnded()
    {
        if (_view != null) 
            _view.OnAppearanceAnimationEnded();
    }

    public void OnHideAnimationEnded()
    {
        if (_view != null) 
            _view.OnHideAnimationEnded();
    }

    private void Reset() => _view = GetComponentInParent<DialogDisplayView>();
}