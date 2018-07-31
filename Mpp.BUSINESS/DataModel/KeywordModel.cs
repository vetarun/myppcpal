using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class KeywordModel
    {
        public long keywordId { get; set; }
        public long adGroupId { get; set; }
        public long campaignId { get; set; }
        public string keywordText { get; set; }
        public string matchType { get; set; }
        public string state { get; set; }
        public double dailyBudget { get; set; }
        public double bid { get; set; }
        public long creationDate { get; set; }
        public long lastUpdatedDate { get; set; }
        public string servingStatus { get; set; }
    }
}