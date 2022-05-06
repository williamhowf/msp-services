using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_MonitorServices_LastActivityLog : BaseEntity //voonkeong 20201124 MDT-1756
    {
        public virtual string ServiceName { get; set; }

        public virtual DateTime CreatedOnUtc { get; set; }
    }
}
