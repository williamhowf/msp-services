using Newtonsoft.Json;

namespace Rmq.Core.Model.Registration
{
    public class RegistrationConsumerDto //wailiang 20200808 MDT-1581
    {
        public RegistrationConsumerDto()
        {
            ParentId = -1; //WilliamHo 20181030 initialize the default value, once referral guid identified, parentid will be replaced
        }

        /// <summary>
        /// Gets or sets the Introducer Global Guid
        /// </summary>
        [JsonProperty("IntroducerGlobalGUID")]
        public string IntroducerGlobalGUID { get; set; }

        /// <summary>
        /// Gets or sets the Global Guid
        /// </summary>
        [JsonProperty("GlobalGUID")]
        public string GlobalGUID { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [JsonProperty("Username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the Email
        /// </summary>
        [JsonProperty("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the UserRole
        /// </summary>
        [JsonProperty("UserRole")]
        public string UserRole { get; set; }

        /// <summary>
        /// Gets or sets the IsUSCitizen
        /// </summary>
        [JsonProperty("IsUSCitizen")]
        public bool IsUSCitizen { get; set; }

        /// <summary>
        /// Gets or sets the ParentId
        /// </summary>
        [JsonProperty("ParentId")]
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the AuthTokenValid
        /// </summary>
        [JsonIgnore]
        public bool AuthTokenValid { get; set; }

        /// <summary>
        /// Gets or sets the ErrorMessage
        /// </summary>
        [JsonIgnore]
        public string ErrorMessage { get; set; }
    }
}
