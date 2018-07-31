using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.UTILITIES.BO
{
    public class UpdateUserInfo
    {
        public int UserId { get; set; }
        public string CustId { get; set; }
        public string CardId { get; set; }
        public int PlanId { get; set; }
        public string Amount { get; set; }
        public  string EndedAt  { get; set; }
        public string TrialEndDate { get; set; }
        public string PlanStartDate { get; set; }
        public string PlanEndDate { get; set; }
      
    }
}