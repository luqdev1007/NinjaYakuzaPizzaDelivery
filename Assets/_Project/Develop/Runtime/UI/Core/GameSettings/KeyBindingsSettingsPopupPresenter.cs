using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class KeyBindingsSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly KeyBindingsSettingsPopupView _view;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        protected override PopupViewBase PopupView => _view;

        public KeyBindingsSettingsPopupPresenter
            (KeyBindingsSettingsPopupView view,
            ICoroutinesPerformer coroutinesPerformer) : base(coroutinesPerformer)
        {
            _view = view;
            _coroutinesPerformer = coroutinesPerformer;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

   
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}