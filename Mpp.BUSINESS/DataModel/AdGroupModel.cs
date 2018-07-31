using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class AdGroupModel
    {
        public long adGroupId { get; set; }
        public long campaignId { get; set; }
        public string name { get; set; }
        public string state { get; set; }
        public double dailyBudget { get; set; }
        public double defaultBid { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int creationDate { get; set; }
        public int lastUpdatedDate { get; set; }
    }
}