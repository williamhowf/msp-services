using Com.GGIT.Database;
using Com.GGIT.LogLib;
using Com.GGIT.Common;
using Rmq.Core.Model.Registration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Rmq.Core.Services.Registration.Consumer
{
    public class RegistrationMemberInsert //wailiang 20200808 MDT-1581
    {
        public RegistrationPublisherDto Insert(RegistrationConsumerDto model)
        {
            var sessionFactory = new SessionDB();
            var returnDto = new RegistrationPublisherDto();
            if (model != null)
            {
                using (var db = sessionFactory.OpenSession())
                {
                    try
                    {
                        IDbCommand command = new SqlCommand();
                        command.Connection = db.Connection;

                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "usp_MSP_API_Registration";

                        var paramGlobalGuid = new SqlParameter("@GlobalGUID", model.GlobalGUID);
                        command.Parameters.Add(paramGlobalGuid);
                        var paramIntroGuid = new SqlParameter("@IntroducerGlobalGUID", model.IntroducerGlobalGUID ?? (object)DBNull.Value);
                        command.Parameters.Add(paramIntroGuid);
                        var paramIsUSCitizen = new SqlParameter("@IsUSCitizen", model.IsUSCitizen);
                        command.Parameters.Add(paramIsUSCitizen);
                        var paramUserRole = new SqlParameter("@UserRole", model.UserRole);
                        command.Parameters.Add(paramUserRole);
                        var paramParentId = new SqlParameter("@ParentID", model.ParentId);
                        command.Parameters.Add(paramParentId);

                        #region Output Parameter
                        SqlParameter pReturnCode = new SqlParameter();
                        pReturnCode.ParameterName = "@outReturnCode";
                        pReturnCode.DbType = DbType.Int32;
                        pReturnCode.Direction = ParameterDirection.Output;
                        command.Parameters.Add(pReturnCode);

                        SqlParameter pReturnMessage = new SqlParameter();
                        pReturnMessage.ParameterName = "@outReturnMessage";
                        pReturnMessage.DbType = DbType.String;
                        pReturnMessage.Size = 200;
                        pReturnMessage.Direction = ParameterDirection.Output;
                        command.Parameters.Add(pReturnMessage);

                        SqlParameter pIntroducerGlobalGUID = new SqlParameter();
                        pIntroducerGlobalGUID.ParameterName = "@outIntroducerGlobalGUID";
                        pIntroducerGlobalGUID.DbType = DbType.String;
                        pIntroducerGlobalGUID.Size = -1;
                        pIntroducerGlobalGUID.Direction = ParameterDirection.Output;
                        command.Parameters.Add(pIntroducerGlobalGUID);

                        SqlParameter pUsername = new SqlParameter();
                        pUsername.ParameterName = "@outUsername";
                        pUsername.DbType = DbType.String;
                        pUsername.Size = 10;
                        pUsername.Direction = ParameterDirection.Output;
                        command.Parameters.Add(pUsername);
                        #endregion Output Parameter

                        command.ExecuteNonQuery(); //execute store procedure
                        returnDto = new RegistrationPublisherDto()
                        {
                            GlobalGUID = model.GlobalGUID,
                            IntroducerGlobalGUID = pIntroducerGlobalGUID.Value.ToString(),
                            Username = pUsername.Value.ToString(),
                            ReturnCode = Convert.ToInt32(pReturnCode.Value.ToString()),
                            ReturnMessage = pReturnMessage.Value.ToString()
                        };
                        return returnDto;
                    }
                    catch (Exception ex)
                    {
                        SingletonLogger.Info("UserGlobalGUID: " + model.GlobalGUID + "IntroducerGlobalGUID: " + model.IntroducerGlobalGUID + " => ReturnCode : " + ex.HResult + " Return Message:" + ex.Message);
                        SingletonLogger.Error(ex.ExceptionToString());
                        returnDto.GlobalGUID = model.GlobalGUID;
                        returnDto.IntroducerGlobalGUID = model.IntroducerGlobalGUID;
                        returnDto.ReturnCode = ex.HResult;
                        returnDto.ReturnMessage = ex.Message;
                        return returnDto;
                    }
                }
            }
            return returnDto;
        }
    }
}
