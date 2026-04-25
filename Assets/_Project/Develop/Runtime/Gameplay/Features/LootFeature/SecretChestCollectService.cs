using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class SecretChestCollectService
    {
        private int _collectedChests;
        private int _totalChestsOnLevel;

        public int CollectedCount => _collectedChests;
        public int TotalCount => _totalChestsOnLevel;

        public void Initialize(int total)
        {
            _totalChestsOnLevel = total;
            _collectedChests = 0;
        }

        public void RegisterChestCollected() => _collectedChests++;

        public bool AllChestsCollected() => _collectedChests >= _totalChestsOnLevel && _totalChestsOnLevel > 0;
    }
}
