using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_InterfaceOut_MegopolyMap : ClassMap<MSP_InterfaceOut_Megopoly> //clement 20200816 MDT-1580
    {
        public MSP_InterfaceOut_MegopolyMap()
        {
            Table("MSP_InterfaceOut_Megopoly");
            DynamicUpdate();
            Id(x => x.ID)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);

            Map(x => x.BatchID);

            Map(x => x.CustomerID);

            Map(x => x.GlobalGuid);

            Map(x => x.ConsumptionID);

            Map(x => x.Amount)
                .Precision(26).Scale(8);

            Map(x => x.CreditAmt)
                .Precision(26).Scale(8);

            Map(x => x.Rate)
                .Precision(26).Scale(8);

            Map(x => x.Status)
                .Nullable();

            Map(x => x.SysRemark)
                .Nullable();

            Map(x => x.CreatedOnUtc);

            Map(x => x.UpdatedOnUtc);

            Map(x => x.SentOnUtc)
                .Nullable();
        }
    }
}
