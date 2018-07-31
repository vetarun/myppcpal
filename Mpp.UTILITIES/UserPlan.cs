using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class UserPlan
    {
        public int id { get; set; }
        public Int32 SkuCount { get; set; }
        public Int32 KeyCount { get; set; }
        public String PlanName { get; set; }
        public Decimal Price { get; set; }
        public string NextDate { get; set; }
    }
}
