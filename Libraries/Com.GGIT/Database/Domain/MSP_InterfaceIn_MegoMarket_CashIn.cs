using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_InterfaceIn_MegoMarket_CashIn : BaseEntity  //clement 20200821 MDT-1583
    {
        public virtual string TrxID { get; set; }

        public virtual string ExternalTrxID { get; set; }

        public virtual string GlobalGuid { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual decimal UsdAmount { get; set; }

        public virtual decimal Rate { get; set; }

        public virtual DateTime TrxOnUtc { get; set; }

        public virtual string Status { get; set; }

        public virtual string SysRemark { get; set; }

        public virtual DateTime CreatedOnUtc { get; set; }

        public virtual DateTime? UpdatedOnUtc { get; set; }

        public virtual DateTime? SentOnUtc { get; set; }

        public virtual bool IsProcessed { get; set; }

        public virtual bool IsSent { get; set; }
    }
}
