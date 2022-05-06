using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_CurrencyMap : ClassMap<MSP_Currency>
    {
        public MSP_CurrencyMap()
        {
            Table("MSP_Currency");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.Code).Not.Nullable();
            Map(x => x.Description).Not.Nullable();
            Map(x => x.Status).Not.Nullable();
        }
    }
}
