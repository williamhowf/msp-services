using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_Notification_MessageMap : ClassMap<MSP_Notification_Message> //wailiang 20200826 MDT-1591
    {
        public MSP_Notification_MessageMap()
        {
            Table("MSPLog_MSP_Notification_Message");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.NotificationID).Not.Nullable();
            Map(x => x.Title).Nullable();
            Map(x => x.Body).Nullable();
            Map(x => x.CreatedOnUtc).Not.Nullable();
        }
    }
}
