using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Language : BaseEntity //wailiang 20200826 MDT-1591
    {
        public virtual string LanguageCode { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual string ResourceValue { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual int CreatedBy { get; set; }
    }
}
