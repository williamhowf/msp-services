using Newtonsoft.Json;

namespace Rmq.Core.Model.Registration
{
    public class RegistrationConsumerInput //wailiang 20200808 MDT-1581
    {
        /// <summary>
        /// Gets or sets the id_token
        /// </summary>
        [JsonProperty("id_token")]
        public string id_token { get; set; }

        /// <summary>
        /// Gets or sets the Security Token
        /// </summary>
        [JsonProperty("securityToken")]
        public string SecurityToken { get; set; }
    }
}
