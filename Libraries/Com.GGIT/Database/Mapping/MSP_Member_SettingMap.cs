using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_Member_SettingMap : ClassMap<MSP_Member_Setting>
    {
        public MSP_Member_SettingMap()
        {
            Table("MSP_Member_Setting");
            Id(x => x.Id)
                .Not.Nullable();
            Map(x => x.CustomerID);
            Map(x => x.PRTransferTo);
        }
    }
}
