using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Notification_Message : BaseEntity //wailiang 20200826 MDT-1591
    {
        public virtual int NotificationID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Body { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
    }
}
