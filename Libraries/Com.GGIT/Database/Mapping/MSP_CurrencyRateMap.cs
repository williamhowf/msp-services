using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_CurrencyRateMap : ClassMap<MSP_CurrencyRate>
    {
        public MSP_CurrencyRateMap()
        {
            Table("MSP_CurrencyRate");
            DynamicInsert();
            DynamicUpdate();
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.Code).Not.Nullable();
            Map(x => x.BtcToCcyRate).Precision(26).Scale(8).Not.Nullable();
            Map(x => x.MbtcToCcyRate).Precision(26).Scale(8).Not.Nullable();
            Map(x => x.CcyToMbtcRate).Precision(26).Scale(8).Not.Nullable();
            Map(x => x.Source).Not.Nullable();
            Map(x => x.RateOnUtc).Not.Nullable();
            Map(x => x.CreatedOnUtc).Not.Nullable();
        }
    }
}
