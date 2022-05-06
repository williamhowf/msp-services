﻿using System;

namespace Com.GGIT.Database.Domain
{
    public class MSP_Megopoly_CashIn : BaseEntity  //wailiang 20200811 MDT-1582
    {
        public virtual int InterfaceID { get; set; }
        public virtual int CustomerID { get; set; }
        public virtual int WalletID { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual string Amount_Enc { get; set; }
        public virtual string Status { get; set; }
        public virtual string SysRemark { get; set; }
        public virtual DateTime CreatedOnUtc { get; set; }
        public virtual DateTime UpdatedOnUtc { get; set; }
    }
}
