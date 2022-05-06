using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Interface_Transactions : BaseEntity
    {
        public virtual int PlatformID { get; set; }
        public virtual string PlatformCode { get; set; }
        public virtual string DistTrxID { get; set; }
        public virtual string GlobalUserID { get; set; }
        public virtual string OrderID { get; set; }
        public virtual DateTime OrderDateTimeUTC { get; set; }
        public virtual decimal OrderAmount { get; set; }
        public virtual string GlobalMerchantID { get; set; }
        public virtual string MerchantName { get; set; }
        public virtual decimal? MerchantAmount { get; set; }
        public virtual DateTime CreatedOnUTC { get; set; }
        public virtual bool IsProcessed { get; set; }
        public virtual string Status { get; set; }
        public virtual string SysRemark { get; set; }
        public virtual bool IsMsgSent { get; set; }
        public virtual DateTime? MsgSentOnUTC { get; set; }
        public virtual decimal GuaranteedAmt { get; set; }
        public virtual decimal ConsumptionAmt { get; set; }
        public virtual decimal GrowthRewardAmt { get; set; }
        public virtual decimal ConsumerReferralAmt { get; set; }
        public virtual decimal TotalDistributionAmt { get; set; }
        public virtual decimal UsdToMbtcRate { get; set; }
        public virtual decimal UsdAmt { get; set; }
    }
}
