using Assets._Project.Develop.Runtime.Utilities.DataManagment;
using Assets._Project.Develop.Runtime.Utilities.DataProviders;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Meta.Features.Upgrades
{
    /// <summary>
    /// Перманентный слой профиля: какие тир-апгрейды куплены.
    ///
    /// ЕДИНСТВЕННЫЙ ПИСАТЕЛЬ PlayerData.PurchasedTiers. Никто больше в это поле
    /// не пишет — ни напрямую, ни через свой IDataWriter. Это осознанно строгое
    /// правило: CompletedLevels в этом же сейве пишут ДВА сервиса
    /// (GameStatsService и LevelsProgressionService), порядок их WriteTo зависит
    /// от порядка регистрации в DataProvider._writers, и кто победит — вопрос
    /// удачи. Здесь такого не будет.
    ///
    /// ReadFrom КОПИРУЕТ СОДЕРЖИМОЕ, а не присваивает ссылку. GameStatsService
    /// делает именно присваивание (CompletedLevels = data.CompletedLevels), из-за
    /// чего сервис и сейв-модель начинают делить один список: правка в сервисе
    /// молча мутирует ещё не сохранённые данные. Тут — свой словарь, всегда.
    /// </summary>
    public class PlayerUpgradesService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private readonly Dictionary<string, int> _purchasedTiers = new();

        public PlayerUpgradesService(PlayerDataProvider playerDataProvider)
        {
            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public int GetTier(string itemId)
        {
            if (_purchasedTiers.TryGetValue(itemId, out int tier))
                return tier;

            return 0;
        }

        public void IncrementTier(string itemId)
        {
            _purchasedTiers[itemId] = GetTier(itemId) + 1;
        }

        public void ReadFrom(PlayerData data)
        {
            _purchasedTiers.Clear();

            // Сейвы, записанные до появления поля, читаются Newtonsoft'ом как null.
            if (data.PurchasedTiers == null)
                return;

            foreach (KeyValuePair<string, int> purchasedTier in data.PurchasedTiers)
            {
                _purchasedTiers[purchasedTier.Key] = purchasedTier.Value;
            }
        }

        public void WriteTo(PlayerData data)
        {
            if (data.PurchasedTiers == null)
                data.PurchasedTiers = new Dictionary<string, int>();

            data.PurchasedTiers.Clear();

            foreach (KeyValuePair<string, int> purchasedTier in _purchasedTiers)
            {
                data.PurchasedTiers[purchasedTier.Key] = purchasedTier.Value;
            }
        }
    }
}
