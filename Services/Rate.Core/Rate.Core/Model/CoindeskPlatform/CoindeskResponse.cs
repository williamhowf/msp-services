using System.Collections.Generic;

namespace Rate.Core.Model.CoindeskPlatform
{
    public class CoindeskResponse : CoindeskHttpBase
    {
        public CoinRateFormat Data { get; set; }
    }

    public class CoinRateFormat : CoindeskRateBase
    {
        public string Interval { get; set; }
        public List<List<double>> Entries { get; set; }
    }
}
