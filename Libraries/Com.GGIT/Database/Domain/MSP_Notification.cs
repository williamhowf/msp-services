using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Notification : BaseEntity //wailiang 20200826 MDT-1591
    {
        public virtual Guid BatchID { get; set; }
        public virtual int RefID { get; set; }
        public virtual string RefType { get; set; }
        public virtual int CustomerID { get; set; }
        public virtual string GlobalGuid { get; set; }
        public virtual string TemplateCode { get; set; }
        public virtual string ParamValue { get; set; }
        public virtual bool IsSent { get; set; }
        public virtual string SysRemark { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime? UpdatedOnUtc { get; set; }
    }
}
