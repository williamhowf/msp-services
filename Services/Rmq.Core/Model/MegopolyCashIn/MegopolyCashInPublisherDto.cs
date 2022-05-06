using Newtonsoft.Json;

namespace Rmq.Core.Model.MegopolyCashIn
{
    public class MegopolyCashInPublisherDto  //wailiang 20200811 MDT-1582
    {
        /// <summary>
        /// aceToken
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// MSP_InterfaceIn_Megopoly_CashIn - GlobalGuid
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// MSP_InterfaceIn_Megopoly_CashIn - TrxID
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// MSP_InterfaceIn_Megopoly_CashIn - Status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// MSP_InterfaceIn_Megopoly_CashIn - SysRemark
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
