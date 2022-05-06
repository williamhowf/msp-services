using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_MemberTree : BaseEntity
    {
        public virtual int CustomerID { get; set; }
        public virtual int? ParentID { get; set; }
        public virtual string RecommendIDs { get; set; }
        public virtual string GlobalGUID { get; set; }
        public virtual string IntroducerGlobalGUID { get; set; }
        public virtual string UserRole { get; set; }
        public virtual bool IsUSCitizen { get; set; }
        public virtual int Level { get; set; }
        public virtual int? MemberWithdrawalAddress_History_ID { get; set; }
        public virtual string WithdrawalWalletAddress { get; set; }
        public virtual int? DepositWalletAddress_ID { get; set; }
        public virtual string DepositWalletAddress { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime UpdatedOnUtc { get; set; }
        public virtual int UpdatedBy { get; set; }
        public virtual bool IsSync { get; set; }
        public virtual string LanguageCode { get; set; }
        public virtual decimal WithdrawalLimit { get; set; }
        public virtual decimal WithdrawalLimitVIP { get; set; }
        public virtual bool IsWithdrawalEnabled { get; set; }
        public virtual decimal CurrMaxDepositPlanAmt { get; set; }
        public virtual string Role { get; set; }
        public virtual bool IsRoleUpgradeEligible { get; set; }
        public virtual DateTime? RoleUpgradeOnUtc { get; set; }
        public virtual decimal RoleMinDepositMbtc { get; set; }
        public virtual decimal RoleMinConsumptionMbtc { get; set; }
        public virtual decimal RoleMemberDepositMbtc { get; set; }
        public virtual decimal RoleMemberConsumptionMbtc { get; set; }
        public virtual string RoleUpgradeVia { get; set; }
        public virtual bool AgentAgreementPopUp { get; set; }
        public virtual int DepositUpgradeCount { get; set; }
        public virtual bool IsUpgradeAllow { get; set; }
        public virtual bool IsByDefaultIntroducer { get; set; }
        public virtual int CurrDeposit_ID { get; set; }
        public virtual decimal CurrDeposit_Discount_Pct { get; set; }
        public virtual string AgentType { get; set; }
        public virtual decimal ConsumerRefPct { get; set; }
    }
}
