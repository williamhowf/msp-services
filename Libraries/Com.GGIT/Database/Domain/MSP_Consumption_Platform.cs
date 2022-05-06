using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Consumption_Platform : BaseEntity
    {
        public virtual int PlatformID { get; set; }
        public virtual string PlatformCode { get; set; }
        public virtual string PlatformName { get; set; }
        public virtual string Description { get; set; }
        public virtual string URL { get; set; }
        public virtual string Status { get; set; }
        public virtual string Tooltips { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsDeleted { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime UpdatedOnUtc { get; set; }
        public virtual int UpdatedBy { get; set; }
    }
}
