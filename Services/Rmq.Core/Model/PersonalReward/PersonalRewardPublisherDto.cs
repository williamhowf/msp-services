using Newtonsoft.Json;

namespace Rmq.Core.Model.PersonalReward
{
    public class PersonalRewardPublisherDto //clement 20200816 MDT-1580
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
        /// MSP_InterfaceOut_Megopoly - CreditAmt - Will be populated when status == SUCCESS
        /// </summary>
        [JsonProperty("rewardBtc")]
        public decimal? RewardBtc { get; set; }
    }
}