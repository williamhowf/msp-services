using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.LogLib;
using Com.GGIT.Common;
using NHibernate;
using Rmq.Core.Common;
using Rmq.Core.Model.MegopolyCashIn;
using System;
using System.Linq;
using Com.GGIT.Database.Extensions;

namespace Rmq.Core.Services.MegopolyCashIn.Consumer
{
    public class MegopolyCashInTransactionInsert  //wailiang 20200811 MDT-1582
    {
        private MegopolyCashInConsumerDto Model;
        private ISession session;
        private readonly UtilityHelper utilHelper;

        public MegopolyCashInTransactionInsert(MegopolyCashInConsumerDto model, UtilityHelper util)
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
                throw new ArgumentNullException(nameof(MegopolyCashInConsumerDto));

            using (session = new SessionDB().OpenSession()) // OpenSession create a unique database connection
            {
                try
                {
                    // Initialize member informations
                    Member member = (from m in session.Query<MSP_MemberTree>()
                                     where m.GlobalGUID == Model.Guid
                                     select new Member { CustomerID = m.CustomerID }).FirstOrDefault();
                    if (member == null) throw new NullReferenceException(nameof(MSP_MemberTree) + " => member not found.");

                    Wallet wallet = (from w in session.Query<MSP_Wallet>()
                                     where w.CustomerID == member.CustomerID
                                     select new Wallet { Id = w.Id }).FirstOrDefault();
                    if (wallet == null) throw new NullReferenceException(nameof(MSP_Wallet) + " => member's wallet not found.");

                    // Insert into MSP_InterfaceIn_Megopoly_CashIn
                    success = InsertInterfaceInMegopolyCashInTrx(out MSP_InterfaceIn_Megopoly_CashIn InterfaceTrx);
                    if (!success) return success;

                    success = InsertMegopolyCashInTrx(InterfaceTrx, member, wallet, out int MegopolyCashInTrxID);
                    if (!success) return success;

                    SingletonLogger.Info("Guid : \"" + Model.Guid + "\" & transactionId : \"" + Model.TransactionId + "\" has successfully inserted. " +
                        "InterfaceID : [" + InterfaceTrx.Id.ToString() + "] , MegopolyCashInID : [" + MegopolyCashInTrxID.ToString() + "]");
                }
                catch (Exception ex)
                {
                    SingletonLogger.Error(ex.Message);
                    return false;
                }
            }
            return success;
        }

        private bool InsertInterfaceInMegopolyCashInTrx(out MSP_InterfaceIn_Megopoly_CashIn InterfaceInMegopolyCashInTrx)
        {
            var CurrentDatetime = DateTime.UtcNow;

            InterfaceInMegopolyCashInTrx = new MSP_InterfaceIn_Megopoly_CashIn
            {
                TrxID = Model.TransactionId,
                GlobalGuid = Model.Guid,
                Amount = Model.AmountBtc.Value,
                Status = "N",
                TrxOnUtc = Model.TransactionDateTimeOnUtc.Value,
                CreatedOnUtc = CurrentDatetime,
                UpdatedOnUtc = CurrentDatetime
            };
            try
            {
                session.SaveTransaction(InterfaceInMegopolyCashInTrx);
                SingletonLogger.Info("TransactionId : " + Model.TransactionId + " , GlobalGuid : " + Model.Guid + " has been inserted into table MSP_InterfaceIn_Megopoly_CashIn.");
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Failed to insert table MSP_InterfaceIn_Megopoly_CashIn => GlobalGuid : " + Model.Guid + " , TrxID : " + Model.TransactionId);
                SingletonLogger.Error(ex.ExceptionToString());
                return false;
            }
            return true;
        }

        private bool InsertMegopolyCashInTrx(MSP_InterfaceIn_Megopoly_CashIn InterfaceTrx, Member member, Wallet wallet, out int MegopolyCashInTrxID)
        {
            MegopolyCashInTrxID = 0;

            try
            {
                var CurrentDatetime = DateTime.UtcNow;

                MSP_Megopoly_CashIn MegopolyCashInTrx = new MSP_Megopoly_CashIn
                {
                    InterfaceID = InterfaceTrx.Id,
                    CustomerID = member.CustomerID,
                    WalletID = wallet.Id,
                    Amount = Model.AmountBtc.Value,
                    Status = "N",
                    CreatedOnUtc = CurrentDatetime,
                    UpdatedOnUtc = CurrentDatetime
                };
                session.SaveTransaction(MegopolyCashInTrx);
                MegopolyCashInTrxID = MegopolyCashInTrx.Id;

                // Update interface transaction when successfully insert trx into MSP_Megopoly_CashIn table
                SingletonLogger.Info("InterfaceID : " + InterfaceTrx.Id + " , CustomerID : " + member.CustomerID + " has been inserted into table MSP_Megopoly_CashIn.");
                SingletonLogger.Info("Updating IsProcessed = 1 for ID : " + InterfaceTrx.Id + " in table MSP_InterfaceIn_Megopoly_CashIn");
                InterfaceTrx.IsProcessed = true;
                session.UpdateTransaction(InterfaceTrx);
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Failed to insert table MSP_Megopoly_CashIn transaction => InterfaceID : " + InterfaceTrx.Id.ToString() + " , CustomerID : " + member.CustomerID.ToString());
                SingletonLogger.Error(ex.ExceptionToString());
                return false;
            }
            return true;
        }
    }
}
