using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_BE_Subscriber : BaseEntity
    {
        public virtual string said { get; set; }
        public virtual string SystemName { get; set; }
        public virtual string SystemDesc { get; set; }
        public virtual string BusinessNature { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsDeleted { get; set; }
    }
}
