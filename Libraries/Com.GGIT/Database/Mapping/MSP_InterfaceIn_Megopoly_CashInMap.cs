using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_InterfaceIn_Megopoly_CashInMap : ClassMap<MSP_InterfaceIn_Megopoly_CashIn>  //wailiang 20200811 MDT-1582
    {
        public MSP_InterfaceIn_Megopoly_CashInMap()
        {
            Table("MSP_InterfaceIn_Megopoly_CashIn");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.TrxID);
            Map(x => x.GlobalGuid);
            Map(x => x.Amount).Precision(20).Scale(8);
            Map(x => x.Status);
            Map(x => x.TrxOnUtc);
            Map(x => x.SysRemark);
            Map(x => x.CreatedOnUtc);
            Map(x => x.UpdatedOnUtc);
            Map(x => x.SentOnUtc);
            Map(x => x.IsProcessed);
            Map(x => x.IsSent);
        }
    }
}
