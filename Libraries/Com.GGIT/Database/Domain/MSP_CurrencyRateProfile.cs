using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_CurrencyRateProfile : BaseEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual int CurrencyRateID { get; set; }
        public virtual decimal BtcToCcyRate { get; set; }
        public virtual decimal MbtcToCcyRate { get; set; }
        public virtual decimal CcyToMbtcRate { get; set; }
        public virtual string Source { get; set; }
        public virtual DateTime RateOnUtc { get; set; }
        public virtual DateTime UpdatedOnUtc { get; set; }
    }
}
