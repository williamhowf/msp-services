using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{ 
    public class MSP_InterfaceIn_MegoMarket_CashInMap : ClassMap<MSP_InterfaceIn_MegoMarket_CashIn>  //clement 20200821 MDT-1583
    {
        public MSP_InterfaceIn_MegoMarket_CashInMap()
        {
            Table("MSP_InterfaceIn_MegoMarket_CashIn");
            DynamicUpdate();
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.TrxID)
                .Length(50);
            Map(x => x.ExternalTrxID)
                .Length(50);
            Map(x => x.GlobalGuid)
                .Length(50);
            Map(x => x.Amount)
                .Precision(26).Scale(8);
            Map(x => x.UsdAmount)
                .Precision(26).Scale(8);
            Map(x => x.Rate)
                .Precision(26).Scale(8);
            Map(x => x.TrxOnUtc);
            Map(x => x.Status)
                .Length(3)
                .Nullable();
            Map(x => x.SysRemark)
                .Length(200)
                .Nullable();
            Map(x => x.CreatedOnUtc);
            Map(x => x.UpdatedOnUtc);
            Map(x => x.SentOnUtc)
                .Nullable();
            Map(x => x.IsProcessed);
            Map(x => x.IsSent);
        }
    }
}
