using Newtonsoft.Json;
using System;

namespace Rmq.Core.Model.PersonalReward
{
    public class PersonalRewardConsumerDto    //clement 20200816 MDT-1580
    {
        /// <summary>
        /// aceToken
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// MSP_InterfaceOut_Megopoly - GlobalGuid
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// MSP_InterfaceOut_Megopoly - BatchID
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// MSP_InterfaceOut_Megopoly - Status - values SUCCESS,FAILED
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// MSP_InterfaceOut_Megopoly - CreditAmt - Will be populated when status == SUCCESS
        /// </summary>
        [JsonProperty("creditAmount")]
        public decimal? CreditAmount { get; set; }

        /// <summary>
        /// MSP_InterfaceOut_Megopoly - Rate - will be populated when status == SUCCESS
        /// </summary>
        [JsonProperty("rate")]
        public decimal? Rate { get; set; }

        /// <summary>
        /// MSP_InterfaceOut_Megopoly - SysRemark - will be populated when status == FAILED
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}