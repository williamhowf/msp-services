using Com.GGIT.Enumeration;
using System;

namespace Rate.Core.Model
{
    public class StartEndDatetime
    {
        private readonly long NowTicks;
        private readonly DateTime Now;
        public StartEndDatetime()
        {
            Now = DateTime.UtcNow;
            NowTicks = Now.Ticks;
            EndDateTime = new DateTime(NowTicks - (NowTicks % (1000 * 1000 * 10 * 60)));
        }
        public DateTime StartDateTime => EndDateTime.AddMinutes(-1);
        public string StartDateTimeToString => StartDateTime.ToString(DatetimeFormatEnum.yyyyMMddTHHmmWithHyphen.ToValue());
        public DateTime EndDateTime { get; }
        public string EndDateTimeToString => EndDateTime.ToString(DatetimeFormatEnum.yyyyMMddTHHmmWithHyphen.ToValue());
    }
}
