using Newtonsoft.Json;

namespace Rmq.Core.Model.Consumption
{
    public class ConsumptionPublisherDto
    {
        /// <summary>
        /// Gets or sets the DistributionTrxId
        /// </summary>
        [JsonProperty("DistributionTrxId")]
        public string DistributionTrxId { get; set; }

        /// <summary>
        /// Gets or sets the OrderId
        /// </summary>
        [JsonProperty("OrderId")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        [JsonProperty("Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the Message
        /// </summary>
        [JsonProperty("Message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the IsSent
        /// </summary>
        [JsonIgnore]
        [JsonProperty("IsSent")]
        public bool IsSent { get; set; }
    }
}
