using Newtonsoft.Json;
using System;

namespace Rmq.Core.Model.Consumption
{
    public class ConsumptionConsumerDto
    {
        /// <summary>
        /// Gets or sets the Security Token
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// Gets or sets the Subscriber Id
        /// </summary>
        [JsonProperty("SubscriberId")]
        public string SubscriberId { get; set; }

        /// <summary>
        /// Gets or sets the Platform Id
        /// </summary>
        [JsonProperty("PlatformID")]
        public string PlatformId { get; set; }

        /// <summary>
        /// Gets or sets the Global User ID (from SSO)
        /// </summary>
        [JsonProperty("GlobalUserID")]
        public string GlobalUserId { get; set; }

        /// <summary>
        /// Gets or sets the Distribution Trx ID
        /// </summary>
        [JsonProperty("DistributionTrxID")]
        public string DistributionTrxId { get; set; }

        /// <summary>
        /// Gets or sets the Order ID 
        /// </summary>
        [JsonProperty("OrderID")]
        public string OrderId { get; set; }

        /// <summary>
        /// Order Date Time UTC
        /// </summary>
        [JsonProperty("OrderDateTime")]
        public DateTime OrderDateTime { get; set; }

        /// <summary>
        /// Order Amount (mBTC)
        /// </summary>
        [JsonProperty("OrderAmt")]
        public decimal OrderAmt { get; set; }

        /// <summary>
        /// Merchant ID
        /// </summary>
        [JsonProperty("MerchantID")]
        public string MerchantId { get; set; }

        /// <summary>
        /// Merchant Name
        /// </summary>
        [JsonProperty("MerchantName")]
        public string MerchantName { get; set; }

        /// <summary>
        /// Merchant Referral Reward Amount (mBTC)
        /// </summary>
        [JsonProperty("MerchantAmount")]
        public decimal? MerchantAmount { get; set; }

        /// <summary>
        /// PersonalRewardAmount = GuaranteedAmt
        /// </summary>
        [JsonProperty("PersonalRewardAmount")]
        public decimal GuaranteedAmt { get; set; }

        /// <summary>
        /// BTCVoucherAmount = ConsumptionAmt
        /// </summary>
        [JsonProperty("BTCVoucherAmount")]
        public decimal ConsumptionAmt { get; set; }

        /// <summary>
        /// GrowthRewardAmount = GrowthRewardAmt
        /// </summary>
        [JsonProperty("GrowthRewardAmount")]
        public decimal GrowthRewardAmt { get; set; }

        /// <summary>
        /// ConsumerReferral1stGenAmount = ConsumerReferralAmt
        /// </summary>
        [JsonProperty("ConsumerReferral1stGenAmount")]
        public decimal ConsumerReferralAmt { get; set; }

        /// <summary>
        /// DistributionAmount = TotalDistributionAmt
        /// </summary>
        [JsonProperty("DistributionAmount")]
        public decimal TotalDistributionAmt { get; set; }

        /// <summary>
        /// USD To Mbtc Rate
        /// </summary>
        [JsonProperty("UsdToMbtcRate")]
        public decimal UsdToMbtcRate { get; set; }

        /// <summary>
        /// USD Amount
        /// </summary>
        [JsonProperty("PersonalRewardAmountUsd")]
        public decimal UsdAmt { get; set; }
    }
}
