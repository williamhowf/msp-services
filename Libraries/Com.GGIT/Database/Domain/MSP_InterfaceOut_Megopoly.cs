using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_InterfaceOut_Megopoly : BaseEntity //clement 20200816 MDT-1580
    {
        public virtual int ID { get; set; }

        public virtual string BatchID { get; set; }

        public virtual int CustomerID { get; set; }

        public virtual string GlobalGuid { get; set; }

        public virtual int ConsumptionID { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual decimal CreditAmt { get; set; }

        public virtual decimal Rate { get; set; }

        public virtual string Status { get; set; }

        public virtual string SysRemark { get; set; }

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime UpdatedOnUtc { get; set; }

        public virtual DateTime? SentOnUtc { get; set; }
    }
}
