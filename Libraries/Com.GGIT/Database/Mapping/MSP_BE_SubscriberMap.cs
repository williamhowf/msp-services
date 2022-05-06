using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_BE_SubscriberMap : ClassMap<MSP_BE_Subscriber>
    {
        public MSP_BE_SubscriberMap()
        {
            Table("MSP_BE_Subscriber");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.said);
            Map(x => x.SystemName);
            Map(x => x.SystemDesc);
            Map(x => x.BusinessNature);
            Map(x => x.IsActive);
            Map(x => x.IsDeleted);
        }
    }
}
