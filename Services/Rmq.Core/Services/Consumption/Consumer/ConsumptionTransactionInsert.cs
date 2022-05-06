using Com.GGIT.Common;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.Database.Extensions;
using Com.GGIT.LogLib;
using NHibernate;
using Rmq.Core.Common;
using Rmq.Core.Model.Consumption;
using Rmq.Core.Settings;
using System;
using System.Linq;

namespace Rmq.Core.Services.Consumption.Consumer
{
    public class ConsumptionTransactionInsert
    {
        private readonly ConsumptionConsumerDto Model;
        private ISession session;
        private decimal merchantAmount = decimal.Zero;
        private readonly UtilityHelper utilHelper;

        public ConsumptionTransactionInsert(ConsumptionConsumerDto model, UtilityHelper util)
        {
            Model = model;
            utilHelper = util;
        }

        public class Member
        {
            public int CustomerID { get; set; }
            public int? ParentID { get; set; }
            public string RecommendIDs { get; set; }
            public decimal ConsumerRefPct { get; set; }
        }

        public class Wallet
        {
            public int Id { get; set; }
            public decimal DepositBalance { get; set; }
        }

        public class MemberSetting //shijian 20200108 MDT-1267
        {
            public string PRTransferTo { get; set; }
        }

        public bool InsertTransaction()
        {
            bool success = false;
            if (Model == null)
                throw new ArgumentNullException(nameof(ConsumptionConsumerDto));

            using (session = new SessionDB().OpenSession()) // OpenSession create a unique database connection
            {
                try
                {
                    // Initialize member informations
                    Member member = (from m in session.Query<MSP_MemberTree>()
                                     where m.GlobalGUID == Model.GlobalUserId
                                     select new Member { CustomerID = m.CustomerID, ParentID = m.ParentID, RecommendIDs = m.RecommendIDs, ConsumerRefPct = m.ConsumerRefPct }).FirstOrDefault();
                    if (member == null) throw new NullReferenceException(nameof(MSP_MemberTree) + " => member not found.");

                    Wallet wallet = (from w in session.Query<MSP_Wallet>()
                                     where w.CustomerID == member.CustomerID
                                     select new Wallet { Id = w.Id, DepositBalance = w.Deposit  }).FirstOrDefault();
                    if (wallet == null) throw new NullReferenceException(nameof(MSP_Wallet) + " => member's wallet not found.");

                    Member merchant = (from m in session.Query<MSP_MemberTree>()
                                       where m.GlobalGUID == Model.MerchantId
                                       select new Member { CustomerID = m.CustomerID, ParentID = m.ParentID }).FirstOrDefault();

                    merchantAmount = Model.MerchantAmount ?? 0m;

                    MemberSetting membersetting = (from ms in session.Query<MSP_Member_Setting>()
                                                   where ms.CustomerID == member.CustomerID
                                                   select new MemberSetting { PRTransferTo = ms.PRTransferTo }).FirstOrDefault(); //shijian 20200108 MDT-1267

                    // insert into interface
                    success = InsertInterfaceTrx(out MSP_Interface_Transactions InterfaceTrx);
                    if (!success) return success;

                    success = InsertConsumptionTrx(InterfaceTrx, member, wallet, merchant, membersetting, out int ConsumptionTrxId);
                    if (!success) return success;

                    SingletonLogger.Info("DistributionId : \"" + Model.DistributionTrxId + "\" & OrderId : \"" + Model.OrderId + "\" has successfully insert. " +
                        "InterfaceID : [" + InterfaceTrx.Id + "] , ConsumptionID : [" + ConsumptionTrxId + "]");
                }
                catch (Exception ex)
                {
                    SingletonLogger.Error(ex.ExceptionToString());
                    return false;
                }
            }
            return success;
        }

        private bool InsertInterfaceTrx(out MSP_Interface_Transactions InterfaceTrx)
        {
            var CurrentDatetime = DateTime.UtcNow;
            decimal UsdToMbtcRate = utilHelper.TruncateDecimal_ScaleEight(Model.UsdToMbtcRate); //wailiang 20200721 MSP-1694

            InterfaceTrx = new MSP_Interface_Transactions
            {
                PlatformID = utilHelper.GetPlatformIdByPlatformCode(Model.PlatformId),
                PlatformCode = Model.PlatformId,
                GlobalUserID = Model.GlobalUserId,
                DistTrxID = Model.DistributionTrxId,
                OrderID = Model.OrderId,
                OrderAmount = Model.OrderAmt,
                OrderDateTimeUTC = Model.OrderDateTime,
                GlobalMerchantID = Model.MerchantId,
                MerchantName = Model.MerchantName,
                MerchantAmount = merchantAmount,
                CreatedOnUTC = CurrentDatetime,
                Status = "N",
                GuaranteedAmt = Model.GuaranteedAmt,
                ConsumptionAmt = Model.ConsumptionAmt,
                GrowthRewardAmt = Model.GrowthRewardAmt,
                ConsumerReferralAmt = Model.ConsumerReferralAmt,
                TotalDistributionAmt = Model.TotalDistributionAmt,
                UsdToMbtcRate = UsdToMbtcRate, //wailiang 20200721 MSP-1694
                UsdAmt = Model.UsdAmt
            };
            try
            {
                session.SaveTransaction(InterfaceTrx);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Fail to insert interface transaction => distributionId : " + Model.DistributionTrxId);
                SingletonLogger.Error(ex.ExceptionToString());
                return false;
            }
            return true;
        }

        private bool InsertConsumptionTrx(MSP_Interface_Transactions InterfaceTrx, Member member, Wallet wallet, Member merchant, MemberSetting membersetting, out int ConsumptionTrxId)
        {
            int MerchantRefCustomerID = 0;
            int MerchantRefWalletID = 0;
            int MerchantCustomerID = 0;
            ConsumptionTrxId = 0;
            try
            {
                /* WilliamHo 20191115 MDT-1124 \/ */
                /* Default: Member user role = consumer, it allow to do consumption
                 * Member user role = merchant, "AllowMerchant" is TRUE, then only allowed to do consumption. 
                 */
                bool AllowMerchant = MspSettings.ConsumptionAllowMerchant();
                bool ConsumerIsMerchant = (from m in session.Query<MSP_MemberTree>()
                                           where m.GlobalGUID == InterfaceTrx.GlobalUserID
                                           select m.UserRole).FirstOrDefault().EqualsIgnoreCase("m");

                /* WilliamHo 20191115 MDT-1124 /\ */
                if ((!ConsumerIsMerchant) || (AllowMerchant && ConsumerIsMerchant))
                {
                    if (merchant != null)
                    {
                        MerchantCustomerID = merchant.CustomerID;
                        MerchantRefCustomerID = merchant.ParentID ?? 0;
                        MerchantRefWalletID = (from w in session.Query<MSP_Wallet>()
                                               where w.CustomerID == merchant.ParentID
                                               select w).FirstOrDefault()?.Id ?? 0;
                    }

                    // Prepare information for consumption table
                    decimal consumptionAmount = utilHelper.TruncateDecimal_ScaleSix(Model.ConsumptionAmt);
                    decimal guaranteedAmount = utilHelper.TruncateDecimal_ScaleSix(Model.GuaranteedAmt);
                    decimal merchantRefAmount = utilHelper.TruncateDecimal_ScaleSix(merchantAmount);
                    decimal consumerRefAmount = utilHelper.TruncateDecimal_ScaleSix(Model.ConsumerReferralAmt);
                    decimal growthRewardAmount = utilHelper.TruncateDecimal_ScaleSix(Model.GrowthRewardAmt);
                    decimal consumptionTrunProfit = Model.ConsumptionAmt - consumptionAmount;
                    decimal guaranteedTrunProfit = Model.GuaranteedAmt - guaranteedAmount;
                    decimal merchantTrunProfit = merchantAmount - merchantRefAmount;
                    decimal consumerTrunProfit = Model.ConsumerReferralAmt - consumerRefAmount;
                    decimal growthTrunProfit = Model.GrowthRewardAmt - growthRewardAmount;
                    decimal truncateProfit = consumptionTrunProfit + guaranteedTrunProfit + merchantTrunProfit + consumerTrunProfit + growthTrunProfit;
                    DateTime CurrentDatetime = DateTime.UtcNow;

                    /* WilliamHo 20191115 MDT-1506 \/ */

                    /* WilliamHo 20200722 MSP-1679, taking value from msp_setting table due to calculation for DU referral amount will be done in SQL job */
                    //Member DU_Member = (from m in session.Query<MSP_MemberTree>() // member's direct upline (a.k.a parent) // WilliamHo 20200722 MSP-1679
                    //                    where m.CustomerID == member.ParentID
                    //                    select new Member { 
                    //                      CustomerID = m.CustomerID, 
                    //                      ParentID = m.ParentID,
                    //                      RecommendIDs = m.RecommendIDs, 
                    //                      ConsumerRefPct = m.ConsumerRefPct
                    //                    }).FirstOrDefault();
                    decimal consumerRefPctSetting = utilHelper.ConsumerReferral_DU_Pct_Setting(); // WilliamHo 20200722 MSP-1679

                    decimal maxRefPct = utilHelper.ConsumerReferral_MaxAmtPct();
                    MSP_SystemCurrency systemCurrency = utilHelper.SystemCurrencyRate();
                    if (systemCurrency.IsNull()) throw new ArgumentNullException("Invalid System Currency - not found.");

                    decimal consumerRefAmt_DU_raw = consumerRefAmount * consumerRefPctSetting / maxRefPct;
                    decimal consumerRefAmt_DU = utilHelper.TruncateDecimal_ScaleSix(consumerRefAmt_DU_raw);
                    truncateProfit += utilHelper.TruncateDecimal_ScaleEight(consumerRefAmt_DU_raw - consumerRefAmt_DU);
                    decimal consumerRefAmt_Offset = consumerRefAmount - consumerRefAmt_DU;
                    decimal consumptionAmtUSD = utilHelper.TruncateDecimal_ScaleEight(consumptionAmount / InterfaceTrx.UsdToMbtcRate);
                    decimal usdAmt = utilHelper.TruncateDecimal_ScaleEight(Model.UsdAmt);
                    /* WilliamHo 20191115 MDT-1506 /\ */

                    /* WilliamHo 20201108 MDT-1708 \/ */
                    decimal minDepositBalance_PRTransfer = utilHelper.MinDepositBalance_PRTransfer();
                    string personalRewardTransferTo = membersetting.PRTransferTo.EqualsIgnoreCase("MG") && (wallet.DepositBalance >= minDepositBalance_PRTransfer) ? "MG" : "MS";
                    /* WilliamHo 20201108 MDT-1708 /\ */

                    MSP_Consumption ConsumptionTrx = new MSP_Consumption
                    {
                        InterfaceID = InterfaceTrx.Id,
                        CustomerID = member.CustomerID,
                        WalletID = wallet.Id,
                        ParentID = member.ParentID ?? 0,
                        RecommendIDs = member.RecommendIDs,
                        PlatformID = InterfaceTrx.PlatformID,
                        MerchantRefCustomerID = MerchantRefCustomerID,
                        MerchantRefWalletID = MerchantRefWalletID,
                        MerchantCustomerID = MerchantCustomerID,
                        ConsumptionAmt = consumptionAmount,
                        GuaranteedAmt = guaranteedAmount,
                        MerchantReferralAmt = merchantRefAmount,
                        Version = MspSettings.GetVersion(),
                        GrowthRewardAmt = growthRewardAmount,
                        ConsumerReferralAmt = consumerRefAmount,
                        TruncateProfit = truncateProfit,
                        UsdAmt = usdAmt,
                        //PRTransferTo = membersetting.PRTransferTo,                    //WilliamHo 20201108 MDT-1708                        
                        ConsumerRefAmt_DU_Pct = consumerRefPctSetting,
                        ConsumerRefAmt_DU = consumerRefAmt_DU,
                        ConsumerRefAmt_Offset = consumerRefAmt_Offset,
                        UsdToMbtcRate = InterfaceTrx.UsdToMbtcRate,
                        ConsumptionAmtUSD = consumptionAmtUSD,
                        DepositToUSDRate_SettingID = systemCurrency.Id,
                        DepositToUSDRate = systemCurrency.Rate,
                        Status = "N",
                        CreatedOnUtc = CurrentDatetime,
                        CreatedBy = 1,
                        UpdatedOnUtc = CurrentDatetime,
                        UpdatedBy = 1,
                        PRTransferTo = personalRewardTransferTo,                        //WilliamHo 20201108 MDT-1708
                        MinDepositBalance_PRTransfer = minDepositBalance_PRTransfer,    //WilliamHo 20201108 MDT-1708
                        CustomerDepositBalance = wallet.DepositBalance,                 //WilliamHo 20201108 MDT-1708
                        CustomerPRTransfer_Setting = membersetting.PRTransferTo         //WilliamHo 20201108 MDT-1708
                    };
                    session.SaveTransaction(ConsumptionTrx);
                    ConsumptionTrxId = ConsumptionTrx.Id;
                }
                else
                {
                    InterfaceTrx.Status = "S";
                    InterfaceTrx.SysRemark = "Merchant is not allowed to do consumption. No record will insert into MSP_Consumption table.";
                }
                // Update interface transaction when successfully insert trx into consumption table
                InterfaceTrx.IsProcessed = true;
                session.UpdateTransaction(InterfaceTrx);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Fail to insert consumption transaction => interfaceId : " + InterfaceTrx.Id + " , distributionId : " + Model.DistributionTrxId);
                SingletonLogger.Error(ex.ExceptionToString());
                InterfaceTrx.SysRemark = ex.Message;
                InterfaceTrx.Status = "E";
                InterfaceTrx.IsProcessed = false;
                session.UpdateTransaction(InterfaceTrx);
                return false;
            }
            return true;
        }
    }
}
