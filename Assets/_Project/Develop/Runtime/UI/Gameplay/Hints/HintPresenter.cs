using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.Hints
{
    public class HintPresenter : PopupPresenterBase
    {
        private readonly HintView _view;
        private readonly string _message;

        public HintPresenter(HintView view, ICoroutinesPerformer coroutinesPerformer, string message)
            : base(coroutinesPerformer)
        {
            _view = view;
            _message = message;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();
            _view.SetText(_message);
        }
    }
}