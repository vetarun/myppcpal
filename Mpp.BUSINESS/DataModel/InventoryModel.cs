using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class InventoryModel
    {
        public long campaignId { get; set; }
        public long adGroupId { get; set; }
        public long keywordId { get; set; }
        public string query { get; set; }
        public int attributedConversions1dSameSKU { set; get; }
        public decimal attributedSales30d { get; set; }
        public decimal cost { get; set; }
        public int clicks { get; set; }
        public int impressions { get; set; }
    }
}