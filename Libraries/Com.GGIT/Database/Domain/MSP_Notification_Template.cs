using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Notification_Template : BaseEntity //wailiang 20200826 MDT-1591
    {
        public virtual string TemplateCode { get; set; }
        public virtual string Title_EN { get; set; }
        public virtual string Title_CN { get; set; }
        public virtual string Body_EN { get; set; }
        public virtual string Body_CN { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime? UpdatedOnUtc { get; set; }
    }
}
