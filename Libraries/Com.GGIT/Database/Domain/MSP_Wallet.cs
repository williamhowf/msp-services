using System;
using System.Collections.Generic;
using System.Text;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Wallet : BaseEntity
    {
        public virtual int CustomerID { get; set; }
        public virtual decimal Mbtc { get; set; }
        public virtual decimal Mbtc_Float { get; set; }
        public virtual decimal Deposit { get; set; }
        public virtual decimal Consumption { get; set; }
        public virtual decimal Profit_DP { get; set; }
        public virtual decimal Profit_CP { get; set; }
        public virtual decimal Profit_CP_Self { get; set; }
        public virtual decimal Profit_DP_Float { get; set; }
        public virtual decimal Mbtc_Withdrawal_Refund_Total { get; set; }
        public virtual decimal Mbtc_Withdrawal_Total { get; set; }
        public virtual decimal Mbtc_Deposit_Total { get; set; }
        public virtual decimal Deposit_Total { get; set; }
        public virtual decimal Deposit_Return_Total { get; set; }
        public virtual decimal Profit_Total { get; set; }
        public virtual decimal Guaranteed_Total { get; set; }
        public virtual decimal MerchantReferral_Total { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime UpdatedOnUtc { get; set; }
        public virtual bool IsSync { get; set; }
        public virtual decimal CityScore { get; set; }
        public virtual int MemberCityScorePct_ID { get; set; }
        public virtual decimal CityScorePct { get; set; }
        public virtual decimal CityScore_Self { get; set; }
        public virtual int MemberCityScorePct_Self_ID { get; set; }
        public virtual decimal CityScorePct_Self { get; set; }
        public virtual decimal MegapolyPool { get; set; }
        public virtual decimal MegapolyAmt { get; set; }
        public virtual decimal Deposit_Mbtc_Disc { get; set; }
        public virtual decimal DepositBal_LoanOut { get; set; }
        public virtual decimal DepositBal_LoanOut_Mbtc_Disc { get; set; }
        public virtual decimal DepositBal_LoanOut_Mbtc { get; set; }
        public virtual decimal DepositBal_LoanOut_Mbtc_Profit { get; set; }
        public virtual decimal DepositBal_LoanIn { get; set; }
        public virtual decimal DepositBal_LoanIn_Mbtc_Disc { get; set; }
        public virtual decimal DepositBal_LoanIn_Mbtc { get; set; }
        public virtual decimal Deposit_OffsetMbtc { get; set; }
        public virtual decimal Deposit_OffsetMbtc_Return { get; set; }
        public virtual decimal Deposit_OffsetMbtc_Profit { get; set; }
        public virtual decimal Deposit_OffsetMbtc_Profit_Return { get; set; }
        public virtual decimal Deposit_OffsetMbtc_Disc { get; set; }
        public virtual decimal Profit_DP_Overflow_Total { get; set; }
        public virtual decimal Consumption_OffsetMbtc { get; set; }
        public virtual decimal Consumption_OffsetMbtc_Profit { get; set; }
        public virtual decimal Consumption_OffsetMbtc_Disc { get; set; }
        public virtual decimal MbtcPatch { get; set; }
        public virtual decimal DividendAmt { get; set; }
        public virtual decimal RetirementAmt { get; set; }
        public virtual decimal ConsumerReferral_Total { get; set; }
        public virtual decimal GrowthReward_Total { get; set; }
        public virtual decimal MegapolyAmt_Total { get; set; }
        public virtual decimal Deposit_Mbtc_Total { get; set; }
        public virtual decimal Deposit_Mbtc_Return_Total { get; set; }
        public virtual decimal CityCredit { get; set; }
    }
}
