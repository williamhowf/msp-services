using System.Collections.Generic;

namespace Com.GGIT.Security.JsonWebToken
{
    public class JwtKeys
    {
        public string Issuer { get; set; }
        public string Client_Id { get; set; }
        public IList<PublicKeys> Keys { get; set; }
    }
    public class PublicKeys
    {
        public string Kid { get; set; }
        public string Alg { get; set; }
        public string Kty { get; set; }
        public string E { get; set; }
        public string N { get; set; }
        public string Use { get; set; }
    }
}
