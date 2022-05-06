using Com.GGIT.Common.Util;
using System;
using System.Collections.Generic;

namespace Rate.Core.Model.CryptoComparePlatform
{
    public class CryptoCompareResponse
    {
        public RAW Raw { get; set; }
    }

    public class RAW
    {
        public IDictionary<string, CcyType> Btc { get; set; }
    }

    public class CcyType
    {
        public string Market { get; set; }
        public string FromSymbol { get; set; }
        public string ToSymbol { get; set; }
        public decimal Price { get; set; }
        public long LastUpdate { get; set; }
        public string LastMarket { get; set; }
        public DateTime RateOnUtc => DatetimeUtil.FromUnixTimeSeconds(LastUpdate);
    }
}
