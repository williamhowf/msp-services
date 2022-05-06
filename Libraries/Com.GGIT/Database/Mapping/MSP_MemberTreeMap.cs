using Com.GGIT.Database.Domain;
using FluentNHibernate.Mapping;
using NHibernate.Mapping;

namespace Com.GGIT.Database.Mapping
{
    public class MSP_MemberTreeMap : ClassMap<MSP_MemberTree>
    {
        public MSP_MemberTreeMap()
        {
            Table("MSP_MemberTree");
            Id(x => x.Id)
                .Not.Nullable()
                .Unique().GeneratedBy.Identity().UnsavedValue(0);
            Map(x => x.CustomerID);
            Map(x => x.ParentID).Nullable();
            Map(x => x.RecommendIDs);
            Map(x => x.GlobalGUID);
            Map(x => x.IntroducerGlobalGUID);
            Map(x => x.UserRole);
            Map(x => x.IsUSCitizen);
            Map(x => x.Level);
            Map(x => x.MemberWithdrawalAddress_History_ID).Nullable();
            Map(x => x.WithdrawalWalletAddress);
            Map(x => x.DepositWalletAddress_ID).Nullable();
            Map(x => x.DepositWalletAddress);
            Map(x => x.CreatedOnUtc);
            Map(x => x.CreatedBy);
            Map(x => x.UpdatedOnUtc);
            Map(x => x.UpdatedBy);
            Map(x => x.IsSync);
            Map(x => x.LanguageCode);
            Map(x => x.WithdrawalLimit);
            Map(x => x.WithdrawalLimitVIP);
            Map(x => x.IsWithdrawalEnabled);
            Map(x => x.CurrMaxDepositPlanAmt);
            Map(x => x.Role);
            Map(x => x.IsRoleUpgradeEligible);
            Map(x => x.RoleUpgradeOnUtc).Nullable();
            Map(x => x.RoleMinDepositMbtc);
            Map(x => x.RoleMinConsumptionMbtc);
            Map(x => x.RoleMemberDepositMbtc);
            Map(x => x.RoleMemberConsumptionMbtc);
            Map(x => x.RoleUpgradeVia);
            Map(x => x.AgentAgreementPopUp);
            Map(x => x.DepositUpgradeCount);
            Map(x => x.IsUpgradeAllow);
            Map(x => x.IsByDefaultIntroducer);
            Map(x => x.CurrDeposit_ID);
            Map(x => x.CurrDeposit_Discount_Pct);
            Map(x => x.AgentType);
            Map(x => x.ConsumerRefPct);
        }
    }
}
