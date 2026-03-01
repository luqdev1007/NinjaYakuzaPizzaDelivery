using Assets._Project.Develop.Runtime.Utilites.DataManagment;
using Assets._Project.Develop.Runtime.Utilites.DataProviders;

namespace Assets._Project.Develop.Runtime.Meta.Features.ShipUpgrades
{
    public class PlayerMainShipDataProvider : IDataReader<PlayerData>, IDataWriter<PlayerData>
    {
        public PlayerMainShipDataProvider(PlayerDataProvider playerDataProvider)
        {
            playerDataProvider.RegisterWriter(this);
            playerDataProvider.RegisterReader(this);
        }

        public float MaxHealth { get; set; }

        public void ReadFrom(PlayerData data)
        {
            MaxHealth = data.MainShipData.MaxHealth;
        }

        public void WriteTo(PlayerData data)
        {
            data.MainShipData.MaxHealth = MaxHealth;
        }
    }
}
