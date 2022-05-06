using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_LanguageMap : ClassMap<MSP_Language> //wailiang 20200826 MDT-1591
    {
        public MSP_LanguageMap()
        {
            Table("MSP_Language");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.LanguageCode).Default("zh-CN").Not.Nullable();
            Map(x => x.ResourceName).Not.Nullable();
            Map(x => x.ResourceValue).Not.Nullable();
            Map(x => x.CreatedOnUtc).Not.Nullable();
            Map(x => x.CreatedBy).Default("0").Not.Nullable();
        }
    }
}
