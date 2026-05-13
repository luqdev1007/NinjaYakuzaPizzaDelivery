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
        private readonly Dictionary<CurrencyTypes, int> _sessionLoot = new();

        public event Action<CurrencyTypes, int, int> OnCurrencyAdded;

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
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount can't be less than zero");

            return _currencies[type].Value >= amount;
        }

        /// <summary>
        /// Добавляет валюту в текущую сессию. 
        /// Изменения сразу видны в UI, но не сохраняются в PlayerData до вызова CommitSessionLoot.
        /// </summary>
        public void Add(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount can't be less than zero");

            if (amount == 0) return;

            // 1. Записываем в сессионный буфер (наш "неподтвержденный" доход)
            if (!_sessionLoot.ContainsKey(type))
                _sessionLoot[type] = 0;

            _sessionLoot[type] += amount;

            // 2. Обновляем реактивное значение для мгновенного фидбека в UI
            _currencies[type].Value += amount;

            // 3. Уведомляем презентеры
            OnCurrencyAdded?.Invoke(type, amount, _currencies[type].Value);
        }

        public void Spend(CurrencyTypes type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount can't be less than zero");

            if (!IsEnough(type, amount))
                throw new InvalidOperationException($"Not enough {type} currency");

            _currencies[type].Value -= amount;

            // Если мы тратим валюту, которую только что подобрали на уровне,
            // уменьшаем сессионный буфер, чтобы не уйти в минус при откате
            if (_sessionLoot.ContainsKey(type))
            {
                _sessionLoot[type] = Mathf.Max(0, _sessionLoot[type] - amount);
            }

            Debug.Log($"Spend {amount} of {type}, left: {_currencies[type].Value}");
        }

        /// <summary>
        /// Подтверждает собранный лут. Вызывать при успешном завершении уровня.
        /// </summary>
        public void CommitSessionLoot()
        {
            _sessionLoot.Clear();
            Debug.Log("Session loot committed and ready to be saved.");
        }

        /// <summary>
        /// Откатывает собранный за сессию лут. Вызывать при рестарте или выходе в меню.
        /// </summary>
        public void RollbackSessionLoot()
        {
            foreach (var loot in _sessionLoot)
            {
                _currencies[loot.Key].Value -= loot.Value;
            }

            _sessionLoot.Clear();
            Debug.Log("Session loot rolled back.");
        }

        public void ReadFrom(PlayerData data)
        {
            foreach (KeyValuePair<CurrencyTypes, int> currency in data.WalletData)
            {
                if (_currencies.ContainsKey(currency.Key))
                    _currencies[currency.Key].Value = currency.Value;
                else
                    _currencies.Add(currency.Key, new ReactiveVariable<int>(currency.Value));
            }
        }

        public void WriteTo(PlayerData data)
        {
            foreach (KeyValuePair<CurrencyTypes, ReactiveVariable<int>> currency in _currencies)
            {
                int persistentValue = currency.Value.Value;

                // Если есть не подтвержденный лут, вычитаем его из сохранения
                if (_sessionLoot.TryGetValue(currency.Key, out int sessionAmount))
                {
                    persistentValue -= sessionAmount;
                }

                if (data.WalletData.ContainsKey(currency.Key))
                    data.WalletData[currency.Key] = persistentValue;
                else
                    data.WalletData.Add(currency.Key, persistentValue);
            }
        }
    }
}