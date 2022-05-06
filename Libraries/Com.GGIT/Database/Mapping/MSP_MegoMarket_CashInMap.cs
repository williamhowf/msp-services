using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_MegoMarket_CashInMap : ClassMap<MSP_MegoMarket_CashIn> //clement 20200821 MDT-1583
    {
        public MSP_MegoMarket_CashInMap()
        {
            Table("MSP_MegoMarket_CashIn");
            DynamicUpdate();
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.InterfaceID);
            Map(x => x.CustomerID);
            Map(x => x.WalletID);
            Map(x => x.Amount)
                .Precision(26).Scale(8);
            Map(x => x.Amount_Enc)
                .Length(70)
                .Nullable();
            Map(x => x.Status)
                .Length(3)
                .Nullable();
            Map(x => x.SysRemark)
                .Length(200)
                .Nullable();
            Map(x => x.CreatedOnUtc);
            Map(x => x.UpdatedOnUtc);
        }

    }
}
