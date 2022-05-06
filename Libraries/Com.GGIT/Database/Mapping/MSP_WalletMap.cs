using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_WalletMap : ClassMap<MSP_Wallet>
    {
        public MSP_WalletMap()
        {
            Table("MSP_Wallet");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.CustomerID);
            Map(x => x.Mbtc).Precision(26).Scale(8);
            Map(x => x.Mbtc_Float).Precision(26).Scale(8);
            Map(x => x.Deposit).Precision(26).Scale(8);
            Map(x => x.Consumption).Precision(26).Scale(8);
            Map(x => x.Profit_DP).Precision(26).Scale(8);
            Map(x => x.Profit_CP).Precision(26).Scale(8);
            Map(x => x.Profit_CP_Self).Precision(26).Scale(8);
            Map(x => x.Profit_DP_Float).Precision(26).Scale(8);
            Map(x => x.Mbtc_Withdrawal_Refund_Total).Precision(26).Scale(8);
            Map(x => x.Mbtc_Withdrawal_Total).Precision(26).Scale(8);
            Map(x => x.Mbtc_Deposit_Total).Precision(26).Scale(8);
            Map(x => x.Deposit_Total).Precision(26).Scale(8);
            Map(x => x.Deposit_Return_Total).Precision(26).Scale(8);
            Map(x => x.Profit_Total).Precision(26).Scale(8);
            Map(x => x.Guaranteed_Total).Precision(26).Scale(8);
            Map(x => x.MerchantReferral_Total).Precision(26).Scale(8);
            Map(x => x.CreatedOnUtc);
            Map(x => x.UpdatedOnUtc);
            Map(x => x.IsSync);
            Map(x => x.CityScore).Precision(26).Scale(8);
            Map(x => x.MemberCityScorePct_ID);
            Map(x => x.CityScorePct);
            Map(x => x.CityScore_Self).Precision(26).Scale(8);
            Map(x => x.MemberCityScorePct_Self_ID);
            Map(x => x.CityScorePct_Self);
            Map(x => x.MegapolyPool).Precision(26).Scale(8);
            Map(x => x.MegapolyAmt).Precision(26).Scale(8);
            Map(x => x.Deposit_Mbtc_Disc).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanOut).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanOut_Mbtc_Disc).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanOut_Mbtc).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanOut_Mbtc_Profit).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanIn).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanIn_Mbtc_Disc).Precision(26).Scale(8);
            Map(x => x.DepositBal_LoanIn_Mbtc).Precision(26).Scale(8);
            Map(x => x.Deposit_OffsetMbtc).Precision(26).Scale(8);
            Map(x => x.Deposit_OffsetMbtc_Return).Precision(26).Scale(8);
            Map(x => x.Deposit_OffsetMbtc_Profit).Precision(26).Scale(8);
            Map(x => x.Deposit_OffsetMbtc_Profit_Return).Precision(26).Scale(8);
            Map(x => x.Deposit_OffsetMbtc_Disc).Precision(26).Scale(8);
            Map(x => x.Profit_DP_Overflow_Total).Precision(26).Scale(8);
            Map(x => x.Consumption_OffsetMbtc).Precision(26).Scale(8);
            Map(x => x.Consumption_OffsetMbtc_Profit).Precision(26).Scale(8);
            Map(x => x.Consumption_OffsetMbtc_Disc).Precision(26).Scale(8);
            Map(x => x.MbtcPatch).Precision(26).Scale(8);
            Map(x => x.DividendAmt).Precision(26).Scale(8);
            Map(x => x.RetirementAmt).Precision(26).Scale(8);
            Map(x => x.ConsumerReferral_Total).Precision(26).Scale(8);
            Map(x => x.GrowthReward_Total).Precision(26).Scale(8);
            Map(x => x.MegapolyAmt_Total).Precision(26).Scale(8);
            Map(x => x.Deposit_Mbtc_Total).Precision(26).Scale(8);
            Map(x => x.Deposit_Mbtc_Return_Total).Precision(26).Scale(8);
            Map(x => x.CityCredit).Precision(26).Scale(8);
        }
    }
}
