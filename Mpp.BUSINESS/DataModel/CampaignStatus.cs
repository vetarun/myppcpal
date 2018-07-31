using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.BUSINESS.DataModel
{
    public class CampaignStatus
    {
        public Int64 RecordID { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }
    }

    public class CreateCampaignResponse
    {
        public Int64 campaignId { get; set; }
        public int code { get; set; }
        public int details { get; set; }
    }

    public class BSTCampaignResponse
    {
        public Int64 campaignId { get; set; }
        public Int64 NewCampId { get; set; }
        public int userid { get; set; }
    }

}
