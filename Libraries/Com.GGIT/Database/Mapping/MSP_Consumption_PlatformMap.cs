using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_Consumption_PlatformMap : ClassMap<MSP_Consumption_Platform>
    {
        public MSP_Consumption_PlatformMap()
        {
            Table("MSP_Consumption_Platform");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.PlatformID);
            Map(x => x.PlatformCode);
            Map(x => x.PlatformName);
            Map(x => x.Description);
            Map(x => x.URL);
            Map(x => x.Status);
            Map(x => x.Tooltips);
            Map(x => x.IsActive);
            Map(x => x.IsDeleted);
            Map(x => x.CreatedOnUtc);
            Map(x => x.CreatedBy);
            Map(x => x.UpdatedOnUtc);
            Map(x => x.UpdatedBy);
        }
    }
}
