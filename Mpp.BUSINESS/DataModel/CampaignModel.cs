using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class CampaignModel
    {
        public long campaignId { get; set; }
        public string name { get; set; }
        public string campaignType { get; set; }
        public string targetingType { get; set; }
        public string state { get; set; }
        public double dailyBudget { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int creationDate { get; set; }
        public int lastUpdatedDate { get; set; }
        public bool premiumBidAdjustment { get; set; }
    }

    



}