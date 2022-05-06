using Com.GGIT.Common.Util;
using System;

namespace Rate.Core.Model.CoindeskPlatform
{
    public class CoindeskRateDatetime
    {
        public CoindeskRateDatetime(long timestamp, decimal rate)
        {
            Timestamp = timestamp;
            Rate = rate;
        }
        public long Timestamp { get; set; }
        public decimal Rate { get; set; }
        public DateTime RateOnUtc => DatetimeUtil.FromUnixTimeMilliseconds(Timestamp);
    }
}
