using Newtonsoft.Json;
using Rmq.Core.Model.Base;

namespace Rmq.Core.Model.Registration
{
    public class RegistrationPublisherDto : ResultObject //wailiang 20200808 MDT-1581
    {
        public RegistrationPublisherDto()
        {
            Username = string.Empty;
        }

        /// <summary>
        /// Gets or sets the Global Guid
        /// </summary>
        [JsonProperty("GlobalGUID")]
        public string GlobalGUID { get; set; }

        /// <summary>
        /// Gets or sets the Introducer Global Guid
        /// </summary>
        [JsonProperty("IntroducerGlobalGUID")]
        public string IntroducerGlobalGUID { get; set; }

        /// <summary>
        /// Gets or sets the Identity Token JWT
        /// </summary>
        [JsonProperty("Id_Token")]
        public string IdToken { get; set; }

        /// <summary>
        /// Gets or sets the Username
        /// </summary>
        [JsonProperty("Username")]
        public string Username { get; set; }
    }
}
