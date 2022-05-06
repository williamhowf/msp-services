using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_Interface_TransactionsMap : ClassMap<MSP_Interface_Transactions>
    {
        public MSP_Interface_TransactionsMap()
        {
            Table("MSP_Interface_Transactions");
            DynamicUpdate();
            DynamicInsert();
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.PlatformID)
                .Not.Nullable();
            Map(x => x.PlatformCode)
                .Length(20);
            Map(x => x.DistTrxID)
                .Length(100);
            Map(x => x.GlobalUserID)
                .Length(50);
            Map(x => x.OrderID)
                .Length(100);
            Map(x => x.OrderDateTimeUTC);
            Map(x => x.OrderAmount)
                .Precision(26).Scale(8);
            Map(x => x.GlobalMerchantID)
                .Length(50);
            Map(x => x.MerchantName)
                .Length(100);
            Map(x => x.MerchantAmount)
                .Nullable()
                .Precision(26).Scale(8);
            Map(x => x.CreatedOnUTC)
                .Not.Nullable();
            Map(x => x.IsProcessed)
                .Not.Nullable();
            Map(x => x.Status).Length(3);
            Map(x => x.SysRemark)
                .Length(300);
            Map(x => x.IsMsgSent);
            Map(x => x.MsgSentOnUTC)
                .Nullable();
            Map(x => x.GuaranteedAmt).Not.Nullable()
                .Precision(26).Scale(8);
            Map(x => x.ConsumptionAmt).Not.Nullable()
                .Precision(26).Scale(8);
            Map(x => x.GrowthRewardAmt).Not.Nullable()
                .Precision(26).Scale(8);
            Map(x => x.ConsumerReferralAmt).Not.Nullable()
                .Precision(26).Scale(8);
            Map(x => x.TotalDistributionAmt).Not.Nullable()
                .Precision(26).Scale(8);
            Map(x => x.UsdToMbtcRate).Not.Nullable()
                .Precision(20).Scale(8);
            Map(x => x.UsdAmt).Not.Nullable()
                .Precision(26).Scale(8);
        }
    }
}
