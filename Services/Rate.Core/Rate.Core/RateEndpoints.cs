namespace Rate.Core
{
    public class RateEndpoints
    {
        public Coindesk Coindesk { get; set; }
        public CryptoCompare CryptoCompare { get; set; }
    }

    public class Coindesk : Endpoints
    {
        public string ExchangeRate { get; set; }
    }

    public class CryptoCompare : Endpoints
    {
        public string ApiKey { get; set; }
    }

    public class Endpoints
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public string Pattern { get; set; }
        public bool Enable { get; set; }
        public bool Debug { get; set; }
    }
}
