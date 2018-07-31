using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
   public  class RevenueData
    {
        
       
        public string SellerName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get
            {
                if (!string.IsNullOrWhiteSpace(FirstName))
                    return this.FirstName + " " + (!string.IsNullOrWhiteSpace(LastName) ? this.LastName : string.Empty);
                else
                    return " ";
            }
                }
        public Int32 Plan { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PayDate { get
            {
               var payDate =PaymentDate.ToString("MMM dd, yyyy");
                return payDate;
            } }
        public Decimal Payment { get; set; }
 
    }
}
