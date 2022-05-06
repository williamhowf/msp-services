using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_SystemCurrencyMap : ClassMap<MSP_SystemCurrency>
    {
        public MSP_SystemCurrencyMap()
        {
            Table("MSP_SystemCurrency");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.Code);
            Map(x => x.Rate).Precision(20).Scale(8);
            Map(x => x.Status);
        }
    }
}
