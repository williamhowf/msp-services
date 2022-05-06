namespace Com.GGIT.Database.Domain
{
    public class MSP_Currency : BaseEntity
    {
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual string Status { get; set; }
    }
}
