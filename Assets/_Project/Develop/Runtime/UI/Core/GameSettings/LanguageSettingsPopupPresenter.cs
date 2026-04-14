using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class LanguageSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly LanguageSettingsPopupView _view;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        protected override PopupViewBase PopupView => _view;

        public LanguageSettingsPopupPresenter
            (LanguageSettingsPopupView view,
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