using Newtonsoft.Json;

namespace Rmq.Core.Model.PersonalReward
{
    public class TransactionEnquiry_PersonalRewardsMegopolyDto //clement 20200816 MDT-1580
    {
        /// <summary>
        /// aceToken
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// Gets or sets the Guid
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the TransactionId
        /// </summary>
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

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