using Assets._Project.Develop.Runtime.Utilites.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.Core.GameSettings
{
    public class GraphicSettingsPopupPresenter : PopupPresenterBase
    {
        private readonly GraphicSettingsPopupView _view;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        protected override PopupViewBase PopupView => _view;

        public GraphicSettingsPopupPresenter
            (GraphicSettingsPopupView view,
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