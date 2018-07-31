using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mpp.BUSINESS.DataModel
{
    public class SnapModel
    {
        public string snapshotId { get; set; }
        public string recordType { get; set; }
        public string status { get; set; }
        public string statusDetails { get; set; }
        public string location { get; set; }
        public int fileSize { get; set; }
        public int expiration { get; set; }
    }
}