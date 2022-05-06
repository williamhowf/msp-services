namespace Com.GGIT.Database.Domain
{
    public class MSP_SystemCurrency : BaseEntity
    {
        public virtual string Code { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual string Status { get; set; }
    }
}
