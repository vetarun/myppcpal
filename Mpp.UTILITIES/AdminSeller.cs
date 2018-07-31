using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
   public class AdminSeller
    {
        public Int32 SellerID { get; set; }
        public String Stp_CustID { get; set; }
        public String Email { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public Boolean Active { get; set; }
        public String ProfileId { get; set; }
        public Boolean SellerAccess { get; set; }
        public String Plan { get; set; }
        public DateTime StartDate { get; set; }
        public Int32 PlanStatus { get; set; }
    }
}
