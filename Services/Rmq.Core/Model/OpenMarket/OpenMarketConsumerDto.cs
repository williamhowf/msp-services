using Newtonsoft.Json;
using System;

namespace Rmq.Core.Model.OpenMarket
{
    public class OpenMarketConsumerDto  //clement 20200821 MDT-1583
    {
        /// <summary>
        /// aceToken
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// MSP_Interface_MegopolyMarket_CashIn - GlobalGuid
        /// </summary>
        [JsonProperty("sellerGuid")]
        public string SellerGuid { get; set; }

        /// <summary>
        /// MSP_Interface_MegopolyMarket_CashIn - TrxID
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// MSP_Interface_MegopolyMarket_CashIn - TrxOnUtc
        /// </summary>
        [JsonProperty("transactionDateTime")]
        public DateTime? TransactionDateTime { get; set; }

        /// <summary>
        /// MSP_Interface_MegopolyMarket_CashIn - Amount
        /// </summary>
        [JsonProperty("incomeMbtc")]
        public decimal? IncomeMbtc { get; set; }

        /// <summary>
        /// MSP_Interface_MegopolyMarket_CashIn - UsdAmount
        /// </summary>
        [JsonProperty("usdTotal")]
        public decimal? UsdTotal { get; set; }

        /// <summary>
        /// MSP_Interface_MegopolyMarket_CashIn - Rate
        /// </summary>
        [JsonProperty("usdToMbtcRate")]
        public decimal? UsdToMbtcRate { get; set; }
    }
}
