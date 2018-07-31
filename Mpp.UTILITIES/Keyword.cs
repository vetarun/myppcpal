using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.UTILITIES
{
    public class Keyword
    {
        public Int64 RecordID { get; set; }
        public String KeywordName { get; set; }
        public String CampaignName { get; set; }
        public String AdGroupName { get; set; }
        public Int32 Impressions { get; set; }
        public Int32 Clicks { get; set; }
        public Decimal Spend { get; set; }
        public Int32 Orders { get; set; }
        public Decimal Sales { get; set; }
        public Decimal ACoS { get; set; }
        public Decimal CTR { get; set; }
        public Decimal CostPerClick { get; set; }
    }

    public class KeywordReport : Keyword
    {
        public decimal CampaignDailyBudget { get; set; }
        public string CampaignTargetingType { get; set; }
        public decimal Max_Bid { get; set; }
        public string MatchType { get; set; }
        public string CampaignStatus { get; set; }
        public string AdGroupStatus { get; set; }
        public string KeyStatus { get; set; }

    }
}