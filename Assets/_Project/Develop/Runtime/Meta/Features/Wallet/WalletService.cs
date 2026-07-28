using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Meta.Features.Wallet
{
    public class WalletService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private readonly Dictionary<CurrencyTypes, ReactiveVariable<int>> _currencies;

        public event Action<CurrencyTypes, int, int> OnCurrencyAdded;

        /// <summary>
        /// Баланс изменился: (тип, дельта, итог). Дельта отрицательна на списании.
        ///
        /// Отдельный канал от OnCurrencyAdded, потому что списание через
        /// OnCurrencyAdded с отрицательным amount противоречило бы имени события,
        /// а переименовывать существующее — ломать чужие подписки.
        /// Add поднимает оба (OnCurrencyAdded — для обратной совместимости),
        /// TrySpend — только этот.
        /// </summary>
        public event Action<CurrencyTypes, int, int> OnCurrencyChanged;

        public WalletService(
            Dictionary<CurrencyTypes, ReactiveVariable<int>> currencies,
            PlayerDataProvider playerDataProvider)
        {
            _currencies = new Dictionary<CurrencyTypes, ReactiveVariable<int>>(currencies);

            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public List<CurrencyTypes> AvailableCurrencies => _currencies.Keys.ToList();

        public IReadOnlyVariable<int> GetCurrency(CurrencyTypes type) => _currencies[type];

        public bool IsEnough(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            return _currencies[type].Value >= amount;
        }

        public void Add(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (amount == 0)
                return;

            _currencies[type].Value += amount;

            OnCurrencyAdded?.Invoke(type, amount, _currencies[type].Value);
            OnCurrencyChanged?.Invoke(type, amount, _currencies[type].Value);
        }

        public void Spend(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (!IsEnough(type, amount))
                throw new InvalidOperationException($"Not enough {type}");

            _currencies[type].Value -= amount;
        }

        /// <summary>
        /// Списание для пользовательских сценариев (покупка в магазине): нехватка
        /// средств — штатный ответ, а не исключение. Существующий Spend с
        /// InvalidOperationException оставлен как есть для вызывающих, которые
        /// уже проверили IsEnough и считают недостачу багом.
        /// </summary>
        public bool TrySpend(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            if (IsEnough(type, amount) == false)
                return false;

            _currencies[type].Value -= amount;

            OnCurrencyChanged?.Invoke(type, -amount, _currencies[type].Value);

            return true;
        }

        public void ReadFrom(PlayerData data)
        {
            foreach (var currency in data.WalletData)
            {
                if (_currencies.ContainsKey(currency.Key))
                    _currencies[currency.Key].Value = currency.Value;
                else
                    _currencies.Add(currency.Key, new ReactiveVariable<int>(currency.Value));
            }
        }

        public void WriteTo(PlayerData data)
        {
            foreach (var currency in _currencies)
            {
                if (data.WalletData.ContainsKey(currency.Key))
                    data.WalletData[currency.Key] = currency.Value.Value;
                else
                    data.WalletData.Add(currency.Key, currency.Value.Value);
            }
        }
    }
}
