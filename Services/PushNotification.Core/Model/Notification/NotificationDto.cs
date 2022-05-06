using System;

namespace PushNotification.Core.Model.Notification
{
    public class NotificationDto //wailiang 20200826 MDT-1591
    {
        public int Id { get; set; }
        public Guid BatchId { get; set; }
        public int RefId { get; set; }
        public string RefType { get; set; }
        public int CustomerID { get; set; }
        public string LanguageCode { get; set; }
        public string TemplateCode { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ParamValue { get; set; }
        public string GlobalGUID { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
