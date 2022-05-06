using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_SettingMap : ClassMap<MSP_Setting>
    {
        public MSP_SettingMap()
        {
            Table("MSP_Setting");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.SettingKey);
            Map(x => x.SettingValue);
            Map(x => x.Description);
            Map(x => x.Status);
            Map(x => x.CreatedOnUtc);
        }
    }
}
