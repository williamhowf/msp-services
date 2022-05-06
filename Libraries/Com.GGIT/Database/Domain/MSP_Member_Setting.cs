namespace Com.GGIT.Database.Domain
{
    public class MSP_Member_Setting : BaseEntity
    {
        public virtual int CustomerID { get; set; }
        public virtual string PRTransferTo { get; set; }
    }
}
