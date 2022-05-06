using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Consumption : BaseEntity
    {
        public virtual int InterfaceID { get; set; }
        public virtual int CustomerID { get; set; }
        public virtual int WalletID { get; set; }
        public virtual int ParentID { get; set; }
        public virtual string RecommendIDs { get; set; }
        public virtual decimal ConsumptionAmtDistPct { get; set; }
        public virtual decimal ConsumptionAmt { get; set; }
        public virtual string ConsumptionAmt_Enc { get; set; }
        public virtual decimal GuaranteedAmtDistPct { get; set; }
        public virtual decimal GuaranteedAmt { get; set; }
        public virtual string GuaranteedAmt_Enc { get; set; }
        public virtual decimal MerchantReferralAmt { get; set; }
        public virtual string MerchantReferralAmt_Enc { get; set; }
        public virtual decimal TruncateProfit { get; set; }
        public virtual int? PlatformID { get; set; }
        public virtual string Remark { get; set; }
        public virtual string SysRemark { get; set; }
        public virtual Guid? CompletedByBatchID { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime UpdatedOnUtc { get; set; }
        public virtual int UpdatedBy { get; set; }
        public virtual bool IsSync { get; set; }
        public virtual int MerchantRefCustomerID { get; set; }
        public virtual int MerchantRefWalletID { get; set; }
        public virtual int MerchantCustomerID { get; set; }
        public virtual byte Version { get; set; }
        public virtual string RecommendAgentIds { get; set; }
        public virtual decimal Customer_CityScore { get; set; }
        public virtual string Customer_Role { get; set; }
        public virtual int GrowthReward_SettingID { get; set; }
        public virtual decimal GrowthRewardPct { get; set; }
        public virtual decimal GrowthRewardAmt { get; set; }
        public virtual decimal GrowthRewardProfit { get; set; }
        public virtual decimal ConsumerReferralAmt { get; set; }
        public virtual decimal GrowthRewardRemain { get; set; }
        public virtual decimal UsdAmt { get; set; }
        public virtual string PRTransferTo { get; set; }
        public virtual decimal ConsumerRefAmt_DU_Pct { get; set; }
        public virtual decimal ConsumerRefAmt_DU { get; set; }
        public virtual decimal ConsumerRefAmt_Offset { get; set; }
        public virtual decimal UsdToMbtcRate { get; set; }
        public virtual decimal ConsumptionAmtUSD { get; set; }
        public virtual int DepositToUSDRate_SettingID { get; set; }
        public virtual decimal DepositToUSDRate { get; set; }
        public virtual decimal MinDepositBalance_PRTransfer { get; set; }
        public virtual decimal CustomerDepositBalance { get; set; }
        public virtual string CustomerPRTransfer_Setting { get; set; }
    }
}
