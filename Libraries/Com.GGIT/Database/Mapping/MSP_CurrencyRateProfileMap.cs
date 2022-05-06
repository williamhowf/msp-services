using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_CurrencyRateProfileMap : ClassMap<MSP_CurrencyRateProfile>
    { 
        public MSP_CurrencyRateProfileMap()
        {
            Table("MSP_CurrencyRateProfile");
            DynamicInsert();
            DynamicUpdate();
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.Code).Not.Nullable();
            Map(x => x.Description).Not.Nullable();
            Map(x => x.CurrencyRateID).Not.Nullable();
            Map(x => x.BtcToCcyRate).Precision(26).Scale(8).Not.Nullable();
            Map(x => x.MbtcToCcyRate).Precision(26).Scale(8).Not.Nullable();
            Map(x => x.CcyToMbtcRate).Precision(26).Scale(8).Not.Nullable();
            Map(x => x.Source).Not.Nullable();
            Map(x => x.RateOnUtc).Not.Nullable();
            Map(x => x.UpdatedOnUtc).Not.Nullable();
        }
    }
}
