using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment
{
    public class PlayerData : ISaveData
    {
        public Dictionary<CurrencyTypes, int> WalletData;
        public int Wins;
        public int Losses;
        public List<int> CompletedLevels;

        // Отдельный флаг, а не производная от "сейв не существует": Reset() пишет
        // сейв сразу, поэтому проверка отсутствия файла живёт ровно один кадр в
        // GameEntryPoint. Старые сейвы без этого поля Newtonsoft читает как false —
        // интро им покажется один раз.
        public bool IntroSeen;
    }
}
