using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.UI.Core.GameSettings;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class LoadMusicPopupPresenter : PopupPresenterBase
    {
        private readonly LoadMusicPopupView _view;

        public LoadMusicPopupPresenter(
            LoadMusicPopupView view,
            ICoroutinesPerformer coroutinesPerformer) : base(coroutinesPerformer)
        {
            _view = view;
        }

        protected override PopupViewBase PopupView => _view;

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
