using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class Optimize
    {
        public string CampaignTargetingType { get; set; }
        public Int64 RecordID { get; set; }
        public String Name { get; set; }
        public Nullable<Boolean> Status { get; set; }
        public String FormulaName { get; set; }
        public Int32 FormulaID { get; set; }
        public bool IsBestSrchTerm { get; set; }
    }
}
