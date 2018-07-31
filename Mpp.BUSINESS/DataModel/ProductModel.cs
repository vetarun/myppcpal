using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class ProductModel
    {
        public long campaignId { get; set; }
        public long adGroupId { get; set; }
        public long keywordId { get; set; }
        public long adId { get; set; }
        public string name { get; set; }
        public bool premiumBidAdjustment { get; set; }
        public string keywordText { get; set; }
        public string matchType { get; set; }
        public string campaignType { get; set; }
        public string targetingType { get; set; }
        public string state { get; set; }
        public double dailyBudget { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int creationDate { get; set; }
        public int lastUpdatedDate { get; set; }
        public double bid { get; set; }
        public double defaultBid { get; set; }
        public string servingStatus { get; set; }
        public string sku { get; set; }
    }
}