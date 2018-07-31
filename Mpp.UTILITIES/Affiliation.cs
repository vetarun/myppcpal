using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class AffiliationCode
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int Percent { get; set; }
        public int? Duration { get; set; }
        public Decimal? Amount { get; set; }
        public int? Max { get; set; }
        public DateTime? Redeemby { get; set; }
        public Decimal Sales { get; set; }
        public int Subscribers { get; set; }
        public decimal PreDiscountSale { get; set; }
        public decimal AffiliateCommission { get; set; }
        public string CreatedBy { get; set; }
        public bool IsAssigned { get; set; }
        public int RedeemedCount { get; set; }
        public bool IsExpired
        {
            get
            {
                if (this.Redeemby != null)
                {
                    return (Redeemby<DateTime.Now);
                }
                else
                {
                    return false;
                }

            }
        }
    }
}
