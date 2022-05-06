using System;

namespace Com.GGIT.Database.Domain
{
    public class Customer : BaseEntity
    {
        public virtual Guid CustomerGuid { get; set; }
        public virtual string Username { get; set; }
        public virtual string Email { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime? LastLoginDateUtc { get; set; }
        public virtual DateTime LastActivityDateUtc { get; set; }
        public virtual bool IsMember { get; set; }
        public virtual DateTime? AccountLockDateUtc { get; set; }
        public virtual bool IsMemberAccountLock { get; set; }
        public virtual bool IsUSCitizen { get; set; }
        public virtual bool IsSync { get; set; }
        public virtual string LanguageCode { get; set; }
        public virtual bool IsAppUser { get; set; }
        public virtual bool GrcWlcmMsgIsRead { get; set; }
    }
}
