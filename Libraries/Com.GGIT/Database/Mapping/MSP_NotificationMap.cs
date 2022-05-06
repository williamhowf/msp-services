using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_NotificationMap : ClassMap<MSP_Notification> //wailiang 20200826 MDT-1591
    {
        public MSP_NotificationMap()
        {
            Table("MSPLog_MSP_Notification");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.BatchID).Not.Nullable();
            Map(x => x.RefID).Not.Nullable();
            Map(x => x.RefType).Nullable();
            Map(x => x.CustomerID).Not.Nullable();
            Map(x => x.GlobalGuid).Nullable();
            Map(x => x.TemplateCode).Nullable();
            Map(x => x.ParamValue).Nullable();
            Map(x => x.IsSent).Default("0").Not.Nullable();
            Map(x => x.SysRemark).Nullable();
            Map(x => x.CreatedOnUtc).Not.Nullable();
            Map(x => x.UpdatedOnUtc).Nullable();
        }
    }
}
