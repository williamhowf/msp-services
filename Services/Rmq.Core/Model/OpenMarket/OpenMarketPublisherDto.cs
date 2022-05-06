using Newtonsoft.Json;

namespace Rmq.Core.Model.OpenMarket
{
    public class OpenMarketPublisherDto  //clement 20200821 MDT-1583
    {
        /// <summary>
        /// aceToken
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// Gets or sets the SellerGuid
        /// </summary>
        //[JsonProperty("sellerGuid")]
        //public string SellerGuid { get; set; }

        /// <summary>
        /// Gets or sets the TransactionId
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the ExternalTransactionId
        /// </summary>
        [JsonProperty("externalTransactionId")]
        public string ExternalTransactionId { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
