using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;

namespace Assets._Project.Develop.Runtime.UI.Wallet
{
    public class GameplayWalletPresenter : IPresenter
    {
        private readonly WalletService _walletService;
        private readonly WalletHUDView _view; 

        public GameplayWalletPresenter(WalletService walletService, WalletHUDView view)
        {
            _walletService = walletService;
            _view = view;
        }

        public void Initialize()
        {
            // 1. Сразу ставим актуальные значения при старте уровня
            foreach (var type in _walletService.AvailableCurrencies)
            {
                UpdateView(type, 0, _walletService.GetCurrency(type).Value);
            }

            // 2. Подписываемся на динамические изменения
            _walletService.OnCurrencyAdded += UpdateView;
        }

        public void Dispose()
        {
            _walletService.OnCurrencyAdded -= UpdateView;
        }

        private void UpdateView(CurrencyTypes type, int addedAmount, int totalValue)
        {
            // Обновляем текст и запускаем простую анимацию "тряски" (Pulse)
            _view.UpdateCurrency(type, totalValue);

            if (addedAmount > 0)
            {
                _view.PlayCollectEffect(type);
            }
        }
    }
}