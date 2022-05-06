using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_Megopoly_CashInMap : ClassMap<MSP_Megopoly_CashIn>  //wailiang 20200811 MDT-1582
    {
        public MSP_Megopoly_CashInMap()
        {
            Table("MSP_Megopoly_CashIn");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.InterfaceID);
            Map(x => x.CustomerID);
            Map(x => x.WalletID);
            Map(x => x.Amount).Precision(20).Scale(8);
            Map(x => x.Amount_Enc);
            Map(x => x.Status);
            Map(x => x.SysRemark);
            Map(x => x.CreatedOnUtc);
            Map(x => x.UpdatedOnUtc);
        }
    }
}
