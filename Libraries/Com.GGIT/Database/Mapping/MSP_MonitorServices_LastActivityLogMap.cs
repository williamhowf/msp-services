using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_MonitorServices_LastActivityLogMap : ClassMap<MSP_MonitorServices_LastActivityLog> //voonkeong 20201124 MDT-1756
    {
        public MSP_MonitorServices_LastActivityLogMap()
        {
            Table("MSP_MonitorServices_LastActivityLog");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.ServiceName)
                .Length(50);
            Map(x => x.CreatedOnUtc);
        }
    }
}
