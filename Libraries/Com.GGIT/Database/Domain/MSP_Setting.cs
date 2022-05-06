using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Setting : BaseEntity
    {
        public virtual string SettingKey { get; set; }
        public virtual string SettingValue { get; set; }
        public virtual string Description { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
    }
}
