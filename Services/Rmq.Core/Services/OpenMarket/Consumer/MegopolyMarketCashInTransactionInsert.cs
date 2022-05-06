using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.LogLib;
using Com.GGIT.Common;
using NHibernate;
using Rmq.Core.Common;
using Rmq.Core.Model.OpenMarket;
using System;
using System.Linq;
using Com.GGIT.Database.Extensions;

namespace Rmq.Core.Services.OpenMarket.Consumer
{
    public class MegopolyMarketCashInTransactionInsert //clement 20200821 MDT-1583
    {
        private OpenMarketConsumerDto Model;
        private ISession session;
        private readonly UtilityHelper utilHelper;

        public MegopolyMarketCashInTransactionInsert(OpenMarketConsumerDto model, UtilityHelper util)
        {
            Model = model;
            utilHelper = util;
        }

        public class Member
        {
            public int CustomerID { get; set; }
        }

        public class Wallet
        {
            public int Id { get; set; }
        }

        public bool InsertTransaction()
        {
            bool success = false;
            if (Model == null)
                throw new ArgumentNullException(nameof(OpenMarketConsumerDto));

            using (session = new SessionDB().OpenSession()) // OpenSession create a unique database connection
            {
                try
                {
                    // Initialize member informations
                    Member member = (from m in session.Query<MSP_MemberTree>()
                                     where m.GlobalGUID == Model.SellerGuid
                                     select new Member { CustomerID = m.CustomerID }).FirstOrDefault();
                    if (member == null) throw new NullReferenceException(nameof(MSP_MemberTree) + " => member not found.");

                    Wallet wallet = (from w in session.Query<MSP_Wallet>()
                                     where w.CustomerID == member.CustomerID
                                     select new Wallet { Id = w.Id }).FirstOrDefault();
                    if (wallet == null) throw new NullReferenceException(nameof(MSP_Wallet) + " => member's wallet not found.");

                    // Insert into MSP_InterfaceIn_Megopoly_CashIn
                    success = InsertInterfaceInMegopolyMarketCashInTrx(out MSP_InterfaceIn_MegoMarket_CashIn InterfaceTrx);
                    if (!success) return success;

                    success = InsertMegopolyMarketCashInTrx(InterfaceTrx, member, wallet, out int MegopolyMarketCashInTrxID);
                    if (!success) return success;

                    SingletonLogger.Info("Guid : \"" + Model.SellerGuid + "\" & transactionId : \"" + Model.TransactionId + "\" has successfully inserted. " +
                        "InterfaceID : [" + InterfaceTrx.Id.ToString() + "] , MegopolyMarketCashInID : [" + MegopolyMarketCashInTrxID.ToString() + "]");
                }
                catch (Exception ex)
                {
                    SingletonLogger.Error(ex);
                    return false;
                }
            }
            return success;
        }

        private bool InsertInterfaceInMegopolyMarketCashInTrx(out MSP_InterfaceIn_MegoMarket_CashIn InterfaceInMegopolyMarketCashInTrx)
        {
            var CurrentDatetime = DateTime.UtcNow;

            InterfaceInMegopolyMarketCashInTrx = new MSP_InterfaceIn_MegoMarket_CashIn
            {
                TrxID = Model.TransactionId,
                ExternalTrxID = UtilityHelper.GetGuidLowercase(),
                GlobalGuid = Model.SellerGuid,
                Amount = Model.IncomeMbtc.Value,
                UsdAmount = Model.UsdTotal.Value,
                Rate = Model.UsdToMbtcRate.Value,
                TrxOnUtc = Model.TransactionDateTime.Value,
                Status = "N",
                CreatedOnUtc = CurrentDatetime,
                UpdatedOnUtc = CurrentDatetime
            };
            try
            {
                session.SaveTransaction(InterfaceInMegopolyMarketCashInTrx);
                SingletonLogger.Info("TransactionId : " + Model.TransactionId + " , GlobalGuid : " + Model.SellerGuid + " has been inserted into table MSP_InterfaceIn_MegoMarket_CashIn.");
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Failed to insert table MSP_InterfaceIn_MegoMarket_CashIn => GlobalGuid : " + Model.SellerGuid + " , TrxID : " + Model.TransactionId);
                SingletonLogger.Error(ex.Message);
                return false;
            }
            return true;
        }

        private bool InsertMegopolyMarketCashInTrx(MSP_InterfaceIn_MegoMarket_CashIn InterfaceTrx, Member member, Wallet wallet, out int MegopolyMarketCashInTrxID)
        {
            MegopolyMarketCashInTrxID = 0;

            try
            {
                var CurrentDatetime = DateTime.UtcNow;

                MSP_MegoMarket_CashIn MegopolyMarketCashInTrx = new MSP_MegoMarket_CashIn
                {
                    InterfaceID = InterfaceTrx.Id,
                    CustomerID = member.CustomerID,
                    WalletID = wallet.Id,
                    Amount = Model.IncomeMbtc.Value,
                    Status = "N",
                    CreatedOnUtc = CurrentDatetime,
                    UpdatedOnUtc = CurrentDatetime
                };
                session.SaveTransaction(MegopolyMarketCashInTrx);
                MegopolyMarketCashInTrxID = MegopolyMarketCashInTrx.Id;

                // Update interface transaction when successfully insert trx into MSP_MegoMarket_CashIn table
                SingletonLogger.Info("InterfaceID : " + InterfaceTrx.Id + " , CustomerID : " + member.CustomerID + " has been inserted into table MSP_MegoMarket_CashIn.");
                SingletonLogger.Info("Updating IsProcessed = 1 for ID : " + InterfaceTrx.Id + " in table MSP_InterfaceIn_MegoMarket_CashIn");
                InterfaceTrx.IsProcessed = true;
                session.UpdateTransaction(InterfaceTrx);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Failed to insert table MSP_MegoMarket_CashIn transaction => InterfaceID : " + InterfaceTrx.Id.ToString() + " , CustomerID : " + member.CustomerID.ToString());
                SingletonLogger.Error(ex);
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
