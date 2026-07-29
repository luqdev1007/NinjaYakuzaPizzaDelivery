namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    /// <summary>
    /// Аддитивный модификатор без привязки к конкретному стату и к источнику.
    ///
    /// Существующий LootCollectRangeAdditiveBuffEffect считает ровно то же
    /// (baseValue + amount), но переиспользовать его нельзя: он ещё и IBuffEffect,
    /// и его Apply(Entity) жёстко прибит к LootCollectRangeModifiers. Перманентная
    /// покупка из магазина — не бафф: у неё нет таймера, нет иконки в HUD и нет
    /// снятия, поэтому тащить ради неё интерфейс баффа было бы враньём о природе
    /// объекта.
    ///
    /// В какой список стата лечь — решает вызывающий; сам модификатор знает
    /// только своё число.
    /// </summary>
    public class AdditiveStatModifier : IStatModifier
    {
        private readonly float _amount;

        public AdditiveStatModifier(float amount)
        {
            _amount = amount;
        }

        public float Apply(float baseValue)
        {
            return baseValue + _amount;
        }
    }
}
