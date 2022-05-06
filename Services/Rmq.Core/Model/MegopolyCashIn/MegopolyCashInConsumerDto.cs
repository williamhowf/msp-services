using Newtonsoft.Json;
using System;

namespace Rmq.Core.Model.MegopolyCashIn
{
    public class MegopolyCashInConsumerDto  //wailiang 20200811 MDT-1582
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
        /// MSP_InterfaceIn_Megopoly_CashIn - TrxOnUtc
        /// </summary>
        [JsonProperty("transactionDatetimeOnUtc")]
        public DateTime? TransactionDateTimeOnUtc { get; set; }

        /// <summary>
        /// MSP_InterfaceIn_Megopoly_CashIn - Amount
        /// </summary>
        [JsonProperty("amountBtc")]
        public decimal? AmountBtc { get; set; }
    }
}
