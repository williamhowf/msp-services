using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_Notification_TemplateMap : ClassMap<MSP_Notification_Template> //wailiang 20200826 MDT-1591
    {
        public MSP_Notification_TemplateMap()
        {
            Table("MSPLog_MSP_Notification_Template");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.TemplateCode).Nullable();
            Map(x => x.Title_EN).Nullable();
            Map(x => x.Title_CN).Nullable();
            Map(x => x.Body_EN).Nullable();
            Map(x => x.Body_CN).Nullable();
            Map(x => x.CreatedOnUtc).Not.Nullable();
            Map(x => x.UpdatedOnUtc).Nullable();
        }
    }
}
