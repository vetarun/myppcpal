using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class MppSales
    {
        public Decimal CurrentMonthlySales { get; set; }
        public Decimal TotalYearlySales { get; set; }
        public Decimal LastMonthSales { get; set; }
        public Int32 Inactive { get; set; }
        public Int32 PendingAccess { get; set; }
        public Int32 TrailEnd { get; set; }
        public Int32 Unsubscribed { get; set; }
        public Int32 TotalActivePaid { get; set; }
        public Int32 TotalActiveTrail { get; set; }
        public Int32 TotalNewlyPaid{ get; set; }
    }
}
