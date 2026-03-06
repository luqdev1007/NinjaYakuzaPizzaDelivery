using Assets._Project.Develop.Runtime.Utilites.DataProviders;
using Assets._Project.Develop.Runtime.Utilites.DataManagment;
using Assets._Project.Develop.Runtime.Utilites.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Meta.Features.Stats
{
    public class GameStatsService : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        private readonly PlayerDataProvider _playerDataProvider;

        public List<int> CompletedLevels { get; private set; } = new();
        public ReactiveVariable<int> Wins { get; private set; } = new();
        public ReactiveVariable<int> Losses { get; private set; } = new();

        public GameStatsService(PlayerDataProvider playerDataProvider)
        {
            _playerDataProvider = playerDataProvider;

            _playerDataProvider.RegisterWriter(this);
            _playerDataProvider.RegisterReader(this);
        }

        public void RegisterVictory()
        {
            Wins.Value++;
        }

        public void RegisterDefeat()
        {
            Losses.Value++;
        }

        public void ReadFrom(PlayerData data)
        {
            CompletedLevels = data.CompletedLevels;
            Wins.Value = data.Wins;
            Losses.Value = data.Losses;
        }

        public void WriteTo(PlayerData data)
        {
            data.Wins = Wins.Value;
            data.Losses = Losses.Value;
            data.CompletedLevels = CompletedLevels;
        }

        public void Reset()
        {
            CompletedLevels.Clear();
            Wins.Value = 0;
            Losses.Value = 0;
        }
    }
}