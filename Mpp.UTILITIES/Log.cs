using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
   public class Log
    {
       
        public Int32 RevertStatus { get; set; }
        public Int32 PopUpID { get; set; }
        public Int32 LogID { get; set; }
        public String ModifiedOn { get; set; }
        public String ModifiedOn1 { get; set; }
        public String MatchType { get; set; }
        public String Time { get; set; }
        public String CampaignName { get; set; }
        public String KeywordName { get; set; }
        public Int64 KeywordID { get; set; }
        public Int64 CampID { get; set; }
        public Int64 AdGroupID { get; set; }
        public Int32 ReportID { get; set; }
        public Int32 ReasonID { get; set; }
        public String AdGroupName { get; set; }
        public String ModifiedField { get; set; }
        public String OldValue { get; set; }
        public String NewValue { get; set; }
        public String Reason { get; set; }
    }
}
