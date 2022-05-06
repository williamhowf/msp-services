using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.LogLib;
using Com.GGIT.Common;
using NHibernate;
using Rmq.Core.Common;
using Rmq.Core.Model.PersonalReward;
using System;
using System.Linq;
using Com.GGIT.Database.Extensions;

namespace Rmq.Core.Services.PersonalReward.Consumer
{
    class PersonalRewardsMegopolyTransactionUpdate  //clement 20200816 MDT-1580
    {
        private PersonalRewardConsumerDto Model;
        private ISession session;
        private readonly UtilityHelper utilHelper;

        public PersonalRewardsMegopolyTransactionUpdate(PersonalRewardConsumerDto model, UtilityHelper util)
        {
            Model = model;
            utilHelper = util;
        }

        public bool UpdateTransaction()
        {
            bool success = false;
            if (Model == null)
                throw new ArgumentNullException(nameof(PersonalRewardConsumerDto));

            try
            {
                success = UpdateInterfaceOutMegopolyTrx(Model);

                // if update is successful, continue to log below. if false, skip the logging.
                if (!success) return success;

                SingletonLogger.Info("Guid : \"" + Model.Guid + "\" & TransactionId : \"" + Model.TransactionId + "\" has successfully updated.");
            }
            catch (Exception ex)
            {
                SingletonLogger.Error(ex.Message);
                return false;
            }

            return success;
        }

        private bool UpdateInterfaceOutMegopolyTrx(PersonalRewardConsumerDto model)
        {
            var CurrentDatetime = DateTime.UtcNow;

            try
            {
                using (var session = new SessionDB().OpenSession()) // OpenSession create a unique database connection
                {
                    var trxRecord = session.Query<MSP_InterfaceOut_Megopoly>()
                    .Where(record => record.GlobalGuid == model.Guid && record.BatchID == model.TransactionId).FirstOrDefault();

                    if (trxRecord != null)
                    {
                        // log trxRecord (ID, Status, CreditAmt, Rate, UpdatedOnUtc) before UPDATE
                        SingletonLogger.Info($"Before update => ID: {trxRecord.ID} | Status: {trxRecord.Status} | CreditAmt: {trxRecord.CreditAmt} | Rate: {trxRecord.Rate} | " +
                            $"UpdatedOnUtc: {trxRecord.UpdatedOnUtc.ToString()}");

                        if (model.Status == "SUCCESS")
                        {
                            trxRecord.Status = "S";
                            trxRecord.CreditAmt = model.CreditAmount.Value;
                            trxRecord.Rate = model.Rate.Value;
                        }
                        else // model.Status == "FAILED"
                        {
                            trxRecord.Status = "F";
                            trxRecord.SysRemark = string.IsNullOrWhiteSpace(model.ErrorMessage) ? "" : model.ErrorMessage;
                        }

                        trxRecord.UpdatedOnUtc = CurrentDatetime;

                        //Log.Info($"Update => ID: {trxRecord.ID} | BatchID: {trxRecord.BatchID} | GlobalGuid: {trxRecord.GlobalGuid} | Status: {trxRecord.Status} | " +
                        //    $"CreditAmt: {trxRecord.CreditAmt} | Rate: {trxRecord.Rate} | UpdatedOnUtc: {trxRecord.UpdatedOnUtc.ToString()}" );

                        session.UpdateTransaction(trxRecord);

                        LogPostUpdate(trxRecord.ID);

                        return true;
                    }
                    else
                    {
                        SingletonLogger.Error("No record found in table MSP_InterfaceOut_Megopoly => guid : " + model.Guid + " , transactionId : " + model.TransactionId);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Failed to update table MSP_InterfaceOut_Megopoly => guid : " + model.Guid + " , transactionId : " + model.TransactionId);
                SingletonLogger.Error(ex.Message);
                return false;
            }
        }

        private void LogPostUpdate(int trxRecordID)
        {
            try
            {
                using (var session = new SessionDB().OpenSession())
                {
                    var query = session.Query<MSP_InterfaceOut_Megopoly>()
                        .Where(record => record.ID == trxRecordID).FirstOrDefault();

                    if (query != null)
                    {
                        SingletonLogger.Info($"After update => ID: {query.ID} | BatchID: {query.BatchID} | GlobalGuid: {query.GlobalGuid} | Status: {query.Status} | " +
                            $"CreditAmt: {query.CreditAmt} | Rate: {query.Rate} | UpdatedOnUtc: {query.UpdatedOnUtc.ToString()}");
                    }
                    else
                    {
                        SingletonLogger.Error("No record found in table MSP_InterfaceOut_Megopoly => ID : " + trxRecordID.ToString());
                    }

                    session.Close();
                }
            }
            catch (Exception ex)
            {
                SingletonLogger.Error("Failed to query table MSP_InterfaceOut_Megopoly => ID : " + trxRecordID.ToString());
                SingletonLogger.Error(ex);
            }
        }
    }
}
