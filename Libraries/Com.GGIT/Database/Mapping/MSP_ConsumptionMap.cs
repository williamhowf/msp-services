using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_ConsumptionMap : ClassMap<MSP_Consumption>
    {
        public MSP_ConsumptionMap()
        {
            Table("MSP_Consumption");
            DynamicInsert();
            DynamicUpdate();
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.InterfaceID).Not.Nullable();
            Map(x => x.CustomerID).Not.Nullable();
            Map(x => x.WalletID).Not.Nullable();
            Map(x => x.ParentID).Not.Nullable();
            Map(x => x.RecommendIDs).Nullable();
            Map(x => x.ConsumptionAmtDistPct).Not.Nullable().Precision(5).Scale(2);
            Map(x => x.ConsumptionAmt).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.ConsumptionAmt_Enc).Nullable();
            Map(x => x.GuaranteedAmtDistPct);
            Map(x => x.GuaranteedAmt).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.GuaranteedAmt_Enc).Nullable();
            Map(x => x.MerchantReferralAmt).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.MerchantReferralAmt_Enc).Nullable();
            Map(x => x.TruncateProfit).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.PlatformID).Nullable();
            Map(x => x.Remark);
            Map(x => x.SysRemark);
            Map(x => x.CompletedByBatchID);
            Map(x => x.Status);
            Map(x => x.CreatedOnUtc);
            Map(x => x.CreatedBy);
            Map(x => x.UpdatedOnUtc);
            Map(x => x.UpdatedBy);
            Map(x => x.IsSync).Not.Nullable().Default("false");
            Map(x => x.MerchantRefCustomerID);
            Map(x => x.MerchantRefWalletID);
            Map(x => x.MerchantCustomerID);
            Map(x => x.Version).Not.Nullable();
            Map(x => x.RecommendAgentIds);
            Map(x => x.Customer_CityScore).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.Customer_Role).Nullable();
            Map(x => x.GrowthReward_SettingID);
            Map(x => x.GrowthRewardPct).Not.Nullable().Precision(5).Scale(2);
            Map(x => x.GrowthRewardAmt).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.GrowthRewardProfit).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.GrowthRewardRemain).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.ConsumerReferralAmt).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.UsdAmt).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.PRTransferTo).Not.Nullable().Default("MS");
            Map(x => x.ConsumerRefAmt_DU_Pct).Not.Nullable();
            Map(x => x.ConsumerRefAmt_DU).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.ConsumerRefAmt_Offset).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.UsdToMbtcRate).Not.Nullable().Precision(20).Scale(8);
            Map(x => x.ConsumptionAmtUSD).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.DepositToUSDRate_SettingID).Not.Nullable();
            Map(x => x.DepositToUSDRate).Not.Nullable().Precision(20).Scale(8);
            Map(x => x.MinDepositBalance_PRTransfer).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.CustomerDepositBalance).Not.Nullable().Precision(26).Scale(8);
            Map(x => x.CustomerPRTransfer_Setting).Nullable();
        }
    }
}
