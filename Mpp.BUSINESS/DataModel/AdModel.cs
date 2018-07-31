using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS.DataModel
{
    public class AdModel
    {
        public long adId { get; set; }
        public long adGroupId { get; set; }
        public long campaignId { get; set; }
        public string sku { get; set; }
        public string asin { get; set; }
        public string state { get; set; }
        public string servingStatus { get; set; }
    }
}
